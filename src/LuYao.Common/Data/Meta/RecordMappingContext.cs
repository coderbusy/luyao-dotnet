using System;

namespace LuYao.Data.Meta;

/// <summary>
/// 封装单次映射任务的执行上下文，持有 <see cref="RecordMappingOptions"/> 并集中实现
/// DTO ↔ <see cref="RecordRow"/> 的全部映射逻辑。
/// </summary>
/// <remarks>
/// 所有 <see cref="XCopy"/> 重载最终均委托给此类执行，避免逻辑分散。
/// 实例化时即调用 <see cref="RecordMappingOptions.MakeReadOnly"/>，冻结选项。
/// </remarks>
internal sealed class RecordMappingContext
{
    private readonly RecordMappingOptions _options;

    /// <summary>
    /// 使用指定选项创建映射上下文，并立即将选项冻结。
    /// </summary>
    /// <param name="options">映射选项，不可为 null。</param>
    internal RecordMappingContext(RecordMappingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.MakeReadOnly();
    }

    // ─── DTO → RecordRow（写入已有列，不自动建列）────────────────────────────────

    /// <summary>
    /// 将 <paramref name="data"/> 中 <paramref name="type"/> 类型的可读属性值写入
    /// <paramref name="target"/> 对应的列。若列不存在则静默跳过，不会自动建列。
    /// </summary>
    internal void CopyDtoToRow(Type type, object data, RecordRow target)
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

            // 非原生支持：需转换器（属性类型 → 列类型）
            var converter = ResolveWriteConverter(prop, col.Type);
            if (converter == null)
            {
                HandleUnsupportedTypeForWrite(prop);
                continue;
            }
            col.Set(target, converter.Convert(prop.Type, col.Type, prop.GetValue(data)));
        }
    }

    // ─── DTO → RecordRow（自动建列）───────────────────────────────────────────────

    /// <summary>
    /// 将 <paramref name="data"/> 中 <paramref name="type"/> 类型的可读属性值写入
    /// <paramref name="target"/> 对应的列。若列不存在则自动创建后再写入。
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

            // 非原生支持：确定列类型
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

    // ─── RecordRow → DTO ──────────────────────────────────────────────────────────

    /// <summary>
    /// 将 <paramref name="source"/> 行中对应列的值写回 <paramref name="data"/> 的可写属性。
    /// </summary>
    internal void CopyRowToDto(Type type, object data, RecordRow source)
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

            // 列类型与属性类型不同，或属性类型不在原生支持列表：需转换器（列类型 → 属性类型）
            var converter = _options.FindConverter(col.Type, prop.Type)
                         ?? (DefaultRecordConverter.Instance.CanConvert(col.Type, prop.Type)
                             ? DefaultRecordConverter.Instance : null);

            if (converter == null)
            {
                HandleConversionFailure(
                    new NotSupportedException(
                        $"属性 '{prop.Name}' 的类型 '{prop.Type.FullName}' 不受支持，且未注册自定义转换器。"));
                continue;
            }

            TryConvertAndSetValue(data, prop, col.Type, prop.Type, rawValue, converter);
        }
    }

    // ─── 建列（AddFrom）──────────────────────────────────────────────────────────

    /// <summary>
    /// 按 <typeparamref name="T"/> 的可读属性向 <paramref name="columns"/> 追加列定义。
    /// </summary>
    internal void AddColumnsFrom<T>(RecordColumnCollection columns) where T : class
        => AddColumnsFrom(typeof(T), columns);

    /// <summary>
    /// 按 <paramref name="type"/> 的可读属性向 <paramref name="columns"/> 追加列定义。
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

    // ─── 私有：列类型与转换器解析 ─────────────────────────────────────────────────

    /// <summary>
    /// 为非原生支持属性确定目标列类型。
    /// 优先级：[RecordColumnStorage] Attribute > UnsupportedTypeHandling（ConvertToString/Bytes）。
    /// 若均不适用（Skip/Throw）则返回 null。
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
    /// 查找写方向（属性类型 → 列类型）的转换器。
    /// 优先级：options 注册 > DefaultRecordConverter。
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
    /// 调用转换器后再赋值；转换或赋值期间的任何异常均遵循 <see cref="ConversionFailureHandling"/> 策略。
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
                $"属性 '{prop.Name}' 的类型 '{prop.Type.FullName}' 不受支持，无法写入 RecordTable。");
        // Skip：静默忽略
    }

    private void HandleConversionFailure(Exception inner)
    {
        if (_options.ConversionFailureHandling == ConversionFailureHandling.Throw)
            throw inner;
        // Skip：静默忽略
    }

    private static void ThrowMissingConverter(XProp prop, Type colType)
    {
        throw new InvalidOperationException(
            $"属性 '{prop.Name}'（类型 '{prop.Type.FullName}'）声明存储为 '{colType.Name}'，" +
            $"但未在 RecordMappingOptions 中注册对应的 RecordConverter，也不在默认转换器支持范围内。");
    }
}
