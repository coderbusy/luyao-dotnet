using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 提供 <see cref="Record"/>、<see cref="RecordRow"/>、<see cref="RecordQuery"/> 的对象映射扩展方法。
/// </summary>
public static class RecordMappingExtensions
{
    #region AddRow<T> / AddRows<T>

    /// <summary>
    /// 将对象的属性按名称匹配写入 Record 的新行。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="record">目标 Record。</param>
    /// <param name="item">要写入的对象。</param>
    /// <returns>新添加的 <see cref="RecordRow"/>。</returns>
    public static RecordRow AddRow<T>(this Record record, T item)
    {
        return AddRow(record, item, null);
    }

    /// <summary>
    /// 将对象的属性按名称匹配写入 Record 的新行（带映射选项）。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="record">目标 Record。</param>
    /// <param name="item">要写入的对象。</param>
    /// <param name="options">映射选项。</param>
    /// <returns>新添加的 <see cref="RecordRow"/>。</returns>
    public static RecordRow AddRow<T>(this Record record, T item, RecordMappingOptions? options)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        if (item == null) throw new ArgumentNullException(nameof(item));

        var row = record.AddRow();
        RecordMappingEngine.WriteRow(item, record, row.Row, options);
        return row;
    }

    /// <summary>
    /// 批量将对象集合写入 Record。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="record">目标 Record。</param>
    /// <param name="items">要写入的对象集合。</param>
    public static void AddRows<T>(this Record record, IEnumerable<T> items)
    {
        AddRows(record, items, null);
    }

    /// <summary>
    /// 批量将对象集合写入 Record（带映射选项）。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="record">目标 Record。</param>
    /// <param name="items">要写入的对象集合。</param>
    /// <param name="options">映射选项。</param>
    public static void AddRows<T>(this Record record, IEnumerable<T> items, RecordMappingOptions? options)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        if (items == null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            if (item == null) throw new ArgumentNullException(nameof(items), "集合中包含 null 元素。");
            var row = record.AddRow();
            RecordMappingEngine.WriteRow(item, record, row.Row, options);
        }
    }

    #endregion

    #region ToList<T> on Record

    /// <summary>
    /// 将 Record 的所有行映射为对象列表。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="record">源 Record。</param>
    /// <returns>映射后的对象列表。</returns>
    public static List<T> ToList<T>(this Record record)
    {
        return ToList<T>(record, null);
    }

    /// <summary>
    /// 将 Record 的所有行映射为对象列表（带映射选项）。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="record">源 Record。</param>
    /// <param name="options">映射选项。</param>
    /// <returns>映射后的对象列表。</returns>
    public static List<T> ToList<T>(this Record record, RecordMappingOptions? options)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        return RecordMappingEngine.ReadAll<T>(record, options);
    }

    #endregion

    #region To<T> on RecordRow

    /// <summary>
    /// 将当前行映射为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="row">源行。</param>
    /// <returns>映射后的对象。</returns>
    public static T To<T>(this RecordRow row)
    {
        return To<T>(row, null);
    }

    /// <summary>
    /// 将当前行映射为指定类型的对象（带映射选项）。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="row">源行。</param>
    /// <param name="options">映射选项。</param>
    /// <returns>映射后的对象。</returns>
    public static T To<T>(this RecordRow row, RecordMappingOptions? options)
    {
        return RecordMappingEngine.ReadRow<T>(row.Record, row.Row, options);
    }

    #endregion

    #region ToList<T> on RecordQuery

    /// <summary>
    /// 直接从查询物化为对象列表，不产生中间 Record。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="query">源查询。</param>
    /// <returns>映射后的对象列表。</returns>
    public static List<T> ToList<T>(this RecordQuery query)
    {
        return ToList<T>(query, null);
    }

    /// <summary>
    /// 直接从查询物化为对象列表（带映射选项），不产生中间 Record。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    /// <param name="query">源查询。</param>
    /// <param name="options">映射选项。</param>
    /// <returns>映射后的对象列表。</returns>
    public static List<T> ToList<T>(this RecordQuery query, RecordMappingOptions? options)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));
        var record = query.ToRecord();
        return RecordMappingEngine.ReadAll<T>(record, options);
    }

    #endregion
}
