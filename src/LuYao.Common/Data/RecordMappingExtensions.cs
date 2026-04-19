using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        EnsureCanAddMappedRows(record, options);

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
        EnsureCanAddMappedRows(record, options);

        foreach (var item in items)
        {
            if (item == null) throw new ArgumentNullException(nameof(items), "集合中包含 null 元素。");
            var row = record.AddRow();
            RecordMappingEngine.WriteRow(item, record, row.Row, options);
        }
    }

    #endregion

    #region AddColumns<T>

    /// <summary>
    /// 将类型 <typeparamref name="T"/> 的公共可读写属性批量添加为列。
    /// 如果存在任意不支持的属性类型，会立即抛出异常，不会进行部分添加。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="record">目标 Record。</param>
    /// <exception cref="InvalidOperationException">类型不包含公共可读写属性。</exception>
    /// <exception cref="NotSupportedException">属性类型不在 Record 支持的列类型白名单中。</exception>
    public static void AddColumns<T>(this Record record)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(static p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
            .ToArray();
        if (properties.Length == 0)
        {
            throw new InvalidOperationException($"类型 '{typeof(T).FullName}' 不包含公共可读写属性。");
        }

        foreach (var property in properties)
        {
            Helpers.ValidateColumnType(property.PropertyType);
        }

        AddColumns(record, properties);
    }

    /// <summary>
    /// 按属性表达式将类型 <typeparamref name="T"/> 的指定属性添加为列（仅支持直接属性，不支持嵌套属性）。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="record">目标 Record。</param>
    /// <param name="column">第一个属性表达式。</param>
    /// <param name="otherColumns">其他属性表达式。</param>
    public static void AddColumns<T>(
        this Record record,
        Expression<Func<T, object?>> column,
        params Expression<Func<T, object?>>[] otherColumns)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        if (column == null) throw new ArgumentNullException(nameof(column));
        if (otherColumns == null) throw new ArgumentNullException(nameof(otherColumns));

        var properties = new List<PropertyInfo> { GetDirectProperty(column, nameof(column)) };
        properties.AddRange(otherColumns.Select(static selector => GetDirectProperty(selector, nameof(otherColumns))));
        foreach (var property in properties)
        {
            Helpers.ValidateColumnType(property.PropertyType);
        }
        AddColumns(record, properties);
    }

    private static PropertyInfo GetDirectProperty<T>(Expression<Func<T, object?>> selector, string paramName)
    {
        if (selector == null) throw new ArgumentNullException(paramName);

        Expression body = selector.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            body = unary.Operand;

        if (body is not MemberExpression memberExpression
            || memberExpression.Expression is not ParameterExpression
            || memberExpression.Member is not PropertyInfo property
            || !property.CanRead)
        {
            throw new ArgumentException("属性表达式必须是直接属性访问（例如 x => x.Name），且不支持嵌套属性。", paramName);
        }

        return property;
    }

    private static void AddColumns(Record record, IEnumerable<PropertyInfo> properties)
    {
        var propertyList = properties.ToArray();
        ValidateColumnsCanBeAdded(record, propertyList);

        foreach (var property in propertyList)
        {
            record.Columns.Add(property.Name, property.PropertyType);
        }
    }

    private static void ValidateColumnsCanBeAdded(Record record, IReadOnlyList<PropertyInfo> properties)
    {
        var duplicateName = properties
            .GroupBy(static property => property.Name)
            .FirstOrDefault(static group => group.Count() > 1)?.Key;
        if (duplicateName != null)
        {
            throw new InvalidOperationException($"存在重复的列名：{duplicateName}。");
        }

        var conflictingName = properties
            .Select(static property => property.Name)
            .FirstOrDefault(record.Columns.Contains);
        if (conflictingName != null)
        {
            throw new InvalidOperationException($"列“{conflictingName}”已存在，无法重复添加。");
        }
    }

    private static void EnsureCanAddMappedRows(Record record, RecordMappingOptions? options)
    {
        if (record.Columns.Count == 0
            && (options == null || (!options.AutoAddColumns && options.Mapper == null)))
        {
            throw new InvalidOperationException("Record 没有任何列。请先添加列或设置 RecordMappingOptions.AutoAddColumns = true。");
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
