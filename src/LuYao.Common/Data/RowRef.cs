using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 代表一个行
/// </summary>
public readonly struct RowRef
{
    private readonly ColumnTable _table;
    private readonly int _rowIndex;

    /// <summary>
    /// 初始化 <see cref="RowRef"/> 的新实例。
    /// </summary>
    /// <param name="table">所属的 <see cref="ColumnTable"/> 实例。</param>
    /// <param name="rowIndex">行索引。</param>
    public RowRef(ColumnTable table, int rowIndex)
    {
        _table = table;
        _rowIndex = rowIndex;
    }

    /// <summary>
    /// 行号
    /// </summary>
    public int RowIndex => _rowIndex;

    /// <summary>
    /// 隐式转换为行索引（int）。
    /// </summary>
    /// <param name="rowRef">要转换的 <see cref="RowRef"/> 实例。</param>
    /// <returns>该行的索引。</returns>
    public static implicit operator int(RowRef rowRef) => rowRef._rowIndex;

    #region Data
    ///<inheritdoc/>
    public Boolean ToBoolean(Column column) => column.ToBoolean(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Boolean value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Byte ToByte(Column column) => column.ToByte(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Byte value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Char ToChar(Column column) => column.ToChar(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Char value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public DateTime ToDateTime(Column column) => column.ToDateTime(this._rowIndex);
    ///<inheritdoc/>
    public void Set(DateTime value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Decimal ToDecimal(Column column) => column.ToDecimal(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Decimal value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Double ToDouble(Column column) => column.ToDouble(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Double value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Int16 ToInt16(Column column) => column.ToInt16(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Int16 value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Int32 ToInt32(Column column) => column.ToInt32(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Int32 value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Int64 ToInt64(Column column) => column.ToInt64(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Int64 value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public SByte ToSByte(Column column) => column.ToSByte(this._rowIndex);
    ///<inheritdoc/>
    public void Set(SByte value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Single ToSingle(Column column) => column.ToSingle(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Single value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public String ToString(Column column) => column.ToString(this._rowIndex);
    ///<inheritdoc/>
    public void Set(String value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public UInt16 ToUInt16(Column column) => column.ToUInt16(this._rowIndex);
    ///<inheritdoc/>
    public void Set(UInt16 value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public UInt32 ToUInt32(Column column) => column.ToUInt32(this._rowIndex);
    ///<inheritdoc/>
    public void Set(UInt32 value, Column column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public UInt64 ToUInt64(Column column) => column.ToUInt64(this._rowIndex);
    ///<inheritdoc/>
    public void Set(UInt64 value, Column column) => column.Set(value, this._rowIndex);
    #endregion
}
