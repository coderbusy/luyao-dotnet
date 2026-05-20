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
            if (!Helpers.IsSupportedForReading(prop))
            {
                HandleUnsupportedType(prop);
                continue;
            }
            var colName = ColumnNameResolver.Resolve(prop, _options);
            var col = cols.Find(colName);
            if (col == null) continue;
            col.Set(target, prop.GetValue(data));
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
            if (!Helpers.IsSupportedForReading(prop))
            {
                HandleUnsupportedType(prop);
                continue;
            }
            var colName = ColumnNameResolver.Resolve(prop, _options);
            var col = cols.Find(colName) ?? cols.Add(colName, prop.Type);
            col.Set(target, prop.GetValue(data));
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

            // 优先使用自定义转换器
            var converter = _options.GetConverter(prop.Type);
            if (converter == null && !Helpers.IsSupportedForWriting(prop))
            {
                // 无转换器且类型不支持——按失败策略处理
                HandleConversionFailure(
                    new NotSupportedException(
                        $"属性 '{prop.Name}' 的类型 '{prop.Type.FullName}' 不受支持，且未注册自定义转换器。"));
                continue;
            }

            try
            {
                var rawValue = col.Get(source);
                prop.SetValue(data, converter != null ? converter(rawValue) : rawValue);
            }
            catch (Exception ex) when (_options.ConversionFailureHandling == ConversionFailureHandling.Skip)
            {
                // Skip：静默跳过，属性保持默认值
                _ = ex;
            }
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
            if (!Helpers.IsSupportedForReading(prop))
            {
                HandleUnsupportedType(prop);
                continue;
            }
            var colName = ColumnNameResolver.Resolve(prop, _options);
            columns.Add(colName, prop.Type);
        }
    }

    // ─── 私有辅助 ────────────────────────────────────────────────────────────────

    private void HandleUnsupportedType(XProp prop)
    {
        if (_options.UnsupportedTypeHandling == UnsupportedTypeHandling.Throw)
            throw new NotSupportedException(
                $"属性 '{prop.Name}' 的类型 '{prop.Type.FullName}' 不受支持，无法写入 RecordTable。");
        // UnsupportedTypeHandling.Skip：静默忽略
    }

    private void HandleConversionFailure(Exception inner)
    {
        if (_options.ConversionFailureHandling == ConversionFailureHandling.Throw)
            throw inner;
        // ConversionFailureHandling.Skip：静默忽略
    }
}
