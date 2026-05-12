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
        /// 列的基础枚举类型标识（不含可空信息和数组维度）。
        /// </summary>
        public RecordColumnType ColumnType { get; }

        /// <summary>
        /// 列是否为可空类型。
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// 数组维度。0 表示非数组，1 表示一维数组，2 表示二维数组，依此类推。
        /// </summary>
        public int ArrayRank { get; }

        /// <summary>
        /// 列的 CLR 数据类型。
        /// </summary>
        public Type Type => Helpers.GetClrType(ColumnType, IsNullable, ArrayRank);

        internal ColumnDef(string name, RecordColumnType columnType, bool isNullable, int arrayRank = 0)
        {
            Name = name;
            ColumnType = columnType;
            IsNullable = isNullable;
            ArrayRank = arrayRank;
        }
    }
}
