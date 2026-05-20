using System;

namespace LuYao.Data.Meta;

/// <summary>
/// Encapsulates the execution context for a single mapping operation.
/// Holds a <see cref="RecordMappingOptions"/> instance and centralises all
/// DTO ↔ <see cref="RecordRow"/> mapping logic.
/// </summary>
/// <remarks>
/// All <see cref="XCopy"/> overloads ultimately delegate to this class,
/// keeping the logic in one place.
/// The options are frozen via <see cref="RecordMappingOptions.MakeReadOnly"/>
/// at construction time.
/// </remarks>
internal sealed class RecordMappingContext
{
    private readonly RecordMappingOptions _options;

    /// <summary>
    /// Creates a mapping context with the specified options and immediately freezes them.
    /// </summary>
    /// <param name="options">Mapping options; must not be null.</param>
    internal RecordMappingContext(RecordMappingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.MakeReadOnly();
    }

    // ─── DTO → RecordRow (map to existing columns, no auto-create) ───────────────

    /// <summary>
    /// Maps readable properties of <paramref name="type"/> on <paramref name="data"/>
    /// into the corresponding columns of <paramref name="target"/>.
    /// Columns that do not exist are silently skipped; no columns are created.
    /// </summary>
    internal void MapDtoToRow(Type type, object data, RecordRow target)
    {
        var cols = target.Table.Columns;
        foreach (var prop in XProp.GetAll(type))
        {
            if (!prop.CanRead) continue;

            var colName = ColumnNameResolver.Resolve(prop, _options);
            var col = cols.Find(colName);
            if (col == null) continue;

            if (Helpers.IsSupportedForReading(prop))
            {
                col.Set(target, prop.GetValue(data));
                continue;
            }

            // Not natively supported: a converter (property type → column type) is required.
            var converter = ResolveWriteConverter(prop, col.Type);
            if (converter == null)
            {
                HandleUnsupportedTypeForWrite(prop);
                continue;
            }
            col.Set(target, converter.Convert(prop.Type, col.Type, prop.GetValue(data)));
        }
    }

    // ─── DTO → RecordRow (auto-create columns) ───────────────────────────────────

    /// <summary>
    /// Writes readable properties of <paramref name="type"/> on <paramref name="data"/>
    /// into the corresponding columns of <paramref name="target"/>.
    /// Missing columns are created automatically before writing.
    /// </summary>
    internal void WriteDtoToRow(Type type, object data, RecordRow target)
    {
        var cols = target.Table.Columns;
        foreach (var prop in XProp.GetAll(type))
        {
            if (!prop.CanRead) continue;

            if (Helpers.IsSupportedForReading(prop))
            {
                var colName = ColumnNameResolver.Resolve(prop, _options);
                var col = cols.Find(colName) ?? cols.Add(colName, prop.Type);
                col.Set(target, prop.GetValue(data));
                continue;
            }

            // Not natively supported: determine the target column type.
            var colType = ResolveColumnType(prop);
            if (colType == null)
            {
                HandleUnsupportedTypeForWrite(prop);
                continue;
            }
            var converter = ResolveWriteConverter(prop, colType);
            if (converter == null)
            {
                ThrowMissingConverter(prop, colType);
                continue;
            }
            var name = ColumnNameResolver.Resolve(prop, _options);
            var destCol = cols.Find(name) ?? cols.Add(name, colType);
            destCol.Set(target, converter.Convert(prop.Type, colType, prop.GetValue(data)));
        }
    }

    // ─── RecordRow → DTO ─────────────────────────────────────────────────────────

    /// <summary>
    /// Maps column values from <paramref name="source"/> into the writable properties
    /// of <paramref name="data"/>. Columns that do not exist are silently skipped.
    /// </summary>
    internal void MapRowToDto(Type type, object data, RecordRow source)
    {
        var cols = source.Table.Columns;
        foreach (var prop in XProp.GetAll(type))
        {
            if (!prop.CanWrite) continue;

            var colName = ColumnNameResolver.Resolve(prop, _options);
            var col = cols.Find(colName);
            if (col == null) continue;

            var rawValue = col.Get(source);

            if (Helpers.IsSupportedForWriting(prop) && col.Type == prop.Type)
            {
                TrySetValue(data, prop, rawValue);
                continue;
            }

            // Column type differs from property type, or property type is not natively
            // supported: a converter (column type → property type) is required.
            var converter = _options.FindConverter(col.Type, prop.Type)
                         ?? (DefaultRecordConverter.Instance.CanConvert(col.Type, prop.Type)
                             ? DefaultRecordConverter.Instance : null);

            if (converter == null)
            {
                HandleConversionFailure(
                    new NotSupportedException(
                        $"Property '{prop.Name}' has type '{prop.Type.FullName}' which is not supported and no custom converter is registered."));
                continue;
            }

            TryConvertAndSetValue(data, prop, col.Type, prop.Type, rawValue, converter);
        }
    }

    // ─── AddFrom (column definition) ─────────────────────────────────────────────

    /// <summary>
    /// Appends column definitions to <paramref name="columns"/> based on the readable
    /// properties of <typeparamref name="T"/>.
    /// </summary>
    internal void AddColumnsFrom<T>(RecordColumnCollection columns) where T : class
        => AddColumnsFrom(typeof(T), columns);

    /// <summary>
    /// Appends column definitions to <paramref name="columns"/> based on the readable
    /// properties of <paramref name="type"/>.
    /// </summary>
    internal void AddColumnsFrom(Type type, RecordColumnCollection columns)
    {
        foreach (var prop in XProp.GetAll(type))
        {
            if (!prop.CanRead) continue;

            if (Helpers.IsSupportedForReading(prop))
            {
                var colName = ColumnNameResolver.Resolve(prop, _options);
                columns.Add(colName, prop.Type);
                continue;
            }

            var colType = ResolveColumnType(prop);
            if (colType == null)
            {
                HandleUnsupportedTypeForWrite(prop);
                continue;
            }
            var converter = ResolveWriteConverter(prop, colType);
            if (converter == null)
            {
                ThrowMissingConverter(prop, colType);
                continue;
            }
            var name = ColumnNameResolver.Resolve(prop, _options);
            columns.Add(name, colType);
        }
    }

    // ─── Private: column-type and converter resolution ───────────────────────────

    /// <summary>
    /// Determines the target column type for a property that is not natively supported.
    /// Priority: <see cref="RecordColumnStorageAttribute"/> &gt; <see cref="UnsupportedTypeHandling"/>
    /// (ConvertToString / ConvertToBytes). Returns <see langword="null"/> when Skip or Throw applies.
    /// </summary>
    private Type? ResolveColumnType(XProp prop)
    {
        var attr = prop.GetCustomAttribute<RecordColumnStorageAttribute>();
        if (attr != null)
        {
            return attr.Target switch
            {
                RecordColumnStorageTarget.String => typeof(string),
                RecordColumnStorageTarget.Bytes  => typeof(byte[]),
                _                                => null, // Skip
            };
        }

        return _options.UnsupportedTypeHandling switch
        {
            UnsupportedTypeHandling.ConvertToString => typeof(string),
            UnsupportedTypeHandling.ConvertToBytes  => typeof(byte[]),
            _                                       => null,
        };
    }

    /// <summary>
    /// Resolves a write-direction converter (property type → column type).
    /// Priority: options-registered converter &gt; <see cref="DefaultRecordConverter"/>.
    /// </summary>
    private RecordConverter? ResolveWriteConverter(XProp prop, Type colType)
    {
        var converter = _options.FindConverter(prop.Type, colType);
        if (converter != null) return converter;
        if (DefaultRecordConverter.Instance.CanConvert(prop.Type, colType))
            return DefaultRecordConverter.Instance;
        return null;
    }

    private void TrySetValue(object data, XProp prop, object? value)
    {
        try
        {
            prop.SetValue(data, value);
        }
        catch (Exception ex) when (_options.ConversionFailureHandling == ConversionFailureHandling.Skip)
        {
            _ = ex;
        }
    }

    /// <summary>
    /// Invokes the converter then assigns the result; any exception during conversion or
    /// assignment is handled according to the <see cref="ConversionFailureHandling"/> policy.
    /// </summary>
    private void TryConvertAndSetValue(object data, XProp prop, Type sourceType, Type targetType, object? value, RecordConverter converter)
    {
        try
        {
            var converted = converter.Convert(sourceType, targetType, value);
            prop.SetValue(data, converted);
        }
        catch (Exception ex) when (_options.ConversionFailureHandling == ConversionFailureHandling.Skip)
        {
            _ = ex;
        }
    }

    private void HandleUnsupportedTypeForWrite(XProp prop)
    {
        if (_options.UnsupportedTypeHandling == UnsupportedTypeHandling.Throw)
            throw new NotSupportedException(
                $"Property '{prop.Name}' has type '{prop.Type.FullName}' which is not supported and cannot be written to a RecordTable.");
        // Skip: silently ignore
    }

    private void HandleConversionFailure(Exception inner)
    {
        if (_options.ConversionFailureHandling == ConversionFailureHandling.Throw)
            throw inner;
        // Skip: silently ignore
    }

    private static void ThrowMissingConverter(XProp prop, Type colType)
    {
        throw new InvalidOperationException(
            $"Property '{prop.Name}' (type '{prop.Type.FullName}') is declared to be stored as '{colType.Name}', " +
            $"but no matching RecordConverter is registered in RecordMappingOptions and it is not covered by the default converter.");
    }
}
