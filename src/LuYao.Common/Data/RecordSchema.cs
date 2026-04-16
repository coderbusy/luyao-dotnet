using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 表示 Record 的列定义信息（列名 + 类型），用于序列化、传输或 Schema 比较。
/// </summary>
public sealed class RecordSchema
{
    private readonly List<ColumnDef> _columns;

    internal RecordSchema(List<ColumnDef> columns)
    {
        _columns = columns ?? throw new ArgumentNullException(nameof(columns));
    }

    /// <summary>
    /// 获取列定义集合。
    /// </summary>
    public IReadOnlyList<ColumnDef> Columns => _columns;

    /// <summary>
    /// 表示单个列的定义信息。
    /// </summary>
    public sealed class ColumnDef
    {
        /// <summary>
        /// 列名。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 列的枚举类型标识。
        /// </summary>
        public RecordColumnType ColumnType { get; }

        /// <summary>
        /// 列的 CLR 数据类型。
        /// </summary>
        public Type Type => Helpers.GetClrType(ColumnType);

        internal ColumnDef(string name, RecordColumnType columnType)
        {
            Name = name;
            ColumnType = columnType;
        }
    }
}
