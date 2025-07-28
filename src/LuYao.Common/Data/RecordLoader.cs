using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LuYao.Data;

/// <summary>
/// Record 与实体类型之间的静态加载器，使用表达式树实现高性能转换
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public static class RecordLoader<T> where T : class, new()
{

    private sealed class PropertyMapping
    {
        public PropertyInfo Property { get; set; } = null!;
        public string ColumnName { get; set; } = null!;
        public Type PropertyType { get; set; } = null!;

        public Action<RecordRow, RecordColumn, T>? WriteEntity { get; set; }
        public Action<T, RecordRow, RecordColumn>? WriteRow { get; set; }
    }

    private static readonly PropertyMapping[] _mappings;
    static RecordLoader()
    {
        _mappings = CreateMappings();
    }

    private static PropertyMapping[]? CreateMappings()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 将 <see cref="RecordRow"/> 的数据填充到指定的实体对象 <typeparamref name="T"/> 中。
    /// </summary>
    /// <param name="row">包含数据的 <see cref="RecordRow"/> 实例。</param>
    /// <param name="target">要填充数据的目标实体对象。</param>
    public static void Populate(RecordRow row, T target)
    {
        Record re = row.Record;
        foreach (var map in _mappings)
        {
            if (map.WriteEntity == null) continue;
            RecordColumn? col = re.Columns.Find(map.ColumnName);
            if (col == null) continue;
            map.WriteEntity(row, col, target);
        }
    }

    /// <summary>
    /// 将实体类型 <typeparamref name="T"/> 的属性名称写入到 <see cref="Record"/> 的列头中。
    /// </summary>
    /// <param name="re">要写入列头的 <see cref="Record"/> 实例。</param>
    public static void WriteHeader(Record re)
    {
        foreach (var map in _mappings)
        {
            if (map.WriteRow == null) continue;
            re.Columns.Add(map.ColumnName, map.PropertyType);
        }
    }

    /// <summary>
    /// 将指定实体对象 <typeparamref name="T"/> 的数据写入到 <see cref="RecordRow"/> 实例中。
    /// </summary>
    /// <param name="instance">要读取数据的实体对象。</param>
    /// <param name="row">要写入数据的 <see cref="RecordRow"/> 实例。</param>
    public static void WriteData(T instance, RecordRow row)
    {
        Record re = row.Record;
        foreach (var map in _mappings)
        {
            if (map.WriteRow == null) continue;
            RecordColumn? col = re.Columns.Find(map.ColumnName);
            if (col == null) continue;
            map.WriteRow(instance, row, col);
        }
    }
}
