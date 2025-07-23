using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 代表一个行
/// </summary>
public readonly struct RecordRow
{
    private readonly Record _table;
    private readonly int _rowIndex;

    /// <summary>
    /// 初始化 <see cref="RecordRow"/> 的新实例。
    /// </summary>
    /// <param name="table">所属的 <see cref="Record"/> 实例。</param>
    /// <param name="rowIndex">行索引。</param>
    public RecordRow(Record table, int rowIndex)
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
    /// <param name="rowRef">要转换的 <see cref="RecordRow"/> 实例。</param>
    /// <returns>该行的索引。</returns>
    public static implicit operator int(RecordRow rowRef) => rowRef._rowIndex;

    ///<inheritdoc/>
    public void SetValue(Object value, RecordColumn column) => column.SetValue(value, this._rowIndex);

    ///<inheritdoc/>
    public object? GetValue(RecordColumn column) => column.GetValue(this._rowIndex);

    #region Data


    ///<inheritdoc/>
    public Boolean ToBoolean(RecordColumn column) => column.ToBoolean(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Boolean value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Byte ToByte(RecordColumn column) => column.ToByte(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Byte value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Char ToChar(RecordColumn column) => column.ToChar(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Char value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public DateTime ToDateTime(RecordColumn column) => column.ToDateTime(this._rowIndex);
    ///<inheritdoc/>
    public void Set(DateTime value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Decimal ToDecimal(RecordColumn column) => column.ToDecimal(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Decimal value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Double ToDouble(RecordColumn column) => column.ToDouble(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Double value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Int16 ToInt16(RecordColumn column) => column.ToInt16(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Int16 value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Int32 ToInt32(RecordColumn column) => column.ToInt32(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Int32 value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Int64 ToInt64(RecordColumn column) => column.ToInt64(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Int64 value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public SByte ToSByte(RecordColumn column) => column.ToSByte(this._rowIndex);
    ///<inheritdoc/>
    public void Set(SByte value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public Single ToSingle(RecordColumn column) => column.ToSingle(this._rowIndex);
    ///<inheritdoc/>
    public void Set(Single value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public String ToString(RecordColumn column) => column.ToString(this._rowIndex);
    ///<inheritdoc/>
    public void Set(String value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public UInt16 ToUInt16(RecordColumn column) => column.ToUInt16(this._rowIndex);
    ///<inheritdoc/>
    public void Set(UInt16 value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public UInt32 ToUInt32(RecordColumn column) => column.ToUInt32(this._rowIndex);
    ///<inheritdoc/>
    public void Set(UInt32 value, RecordColumn column) => column.Set(value, this._rowIndex);

    ///<inheritdoc/>
    public UInt64 ToUInt64(RecordColumn column) => column.ToUInt64(this._rowIndex);
    ///<inheritdoc/>
    public void Set(UInt64 value, RecordColumn column) => column.Set(value, this._rowIndex);
    #endregion
}
