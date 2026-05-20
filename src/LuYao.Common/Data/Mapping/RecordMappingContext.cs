using LuYao.Data.Meta;
using System;
using LuYao;

namespace LuYao.Data.Mapping;

/// <summary>
/// Encapsulates the execution context for a single mapping operation.
/// Holds a <see cref="RecordMappingOptions"/> instance and centralises all
/// DTO �?<see cref="RecordRow"/> mapping logic.
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

    // ─── DTO �?RecordRow (map to existing columns, no auto-create) ───────────────

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

            HandleUnsupportedTypeForWrite(prop);
        }
    }

    // ─── DTO �?RecordRow (auto-create columns) ───────────────────────────────────

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

            HandleUnsupportedTypeForWrite(prop);
        }
    }

    // ─── RecordRow �?DTO ─────────────────────────────────────────────────────────

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

            if (!Helpers.IsSupportedForWriting(prop))
            {
                HandleConversionFailure(
                    new NotSupportedException(
                        $"Property '{prop.Name}' has type '{prop.Type.FullName}' which is not supported."));
                continue;
            }

            if (col.Type == prop.Type)
            {
                TrySetValue(data, prop, rawValue);
                continue;
            }

            // Types differ (e.g. column is int, DTO property is decimal).
            // Use Convert.ChangeType as a best-effort numeric/primitive coercion,
            // mirroring the strategy used by Dapper and similar micro-ORMs.
            TrySetValueWithConversion(data, prop, rawValue);
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

            HandleUnsupportedTypeForWrite(prop);
        }
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
    /// Attempts to set <paramref name="value"/> on <paramref name="prop"/> after
    /// coercing it to the property's declared type via <see cref="Convert.ChangeType(object,Type)"/>.
    /// Falls back to direct assignment when the value is already assignable (e.g. null or subtype).
    /// Obeys <see cref="RecordMappingOptions.ConversionFailureHandling"/> on failure.
    /// </summary>
    private void TrySetValueWithConversion(object data, XProp prop, object? value)
    {
        try
        {
            if (value is null or DBNull)
            {
                prop.SetValue(data, null);
                return;
            }

            prop.SetValue(data, TypeConvert.ChangeType(value, prop.Type));
        }
        catch (Exception ex)
        {
            HandleConversionFailure(
                new InvalidCastException(
                    $"Cannot convert value '{value}' ({value?.GetType().FullName}) to property '{prop.Name}' of type '{prop.Type.FullName}'.",
                    ex));
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


}
