using System;

namespace LuYao.Data.Columns;

/// <summary>
/// 表示一个用于存储 <see cref="Int32"/> 类型数据的记录列。
/// </summary>
public sealed class Int32RecordColumn : RecordColumn<Int32>
{
    internal Int32RecordColumn(Record record, string name)
        : base(record, name, RecordDataCode.Int32, typeof(Int32))
    {
    }

    #region Set

    /// <inheritdoc/>
    public override void Set(Boolean value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Byte value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Char value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(DateTime value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Decimal value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Double value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Int16 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Int32 value, int row)
    {
        this.OnSet(row);
        this._data[row] = value;
    }

    /// <inheritdoc/>
    public override void Set(Int64 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(SByte value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(Single value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(String value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(UInt16 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(UInt32 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    /// <inheritdoc/>
    public override void Set(UInt64 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToInt32(value);
    }

    #endregion

    #region To

    /// <inheritdoc/>
    public override Boolean GetBoolean(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToBoolean(this._data[row]);
    }

    /// <inheritdoc/>
    public override Byte GetByte(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToByte(this._data[row]);
    }

    /// <inheritdoc/>
    public override Char GetChar(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToChar(this._data[row]);
    }

    /// <inheritdoc/>
    public override DateTime GetDateTime(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToDateTime(this._data[row]);
    }

    /// <inheritdoc/>
    public override Decimal GetDecimal(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToDecimal(this._data[row]);
    }

    /// <inheritdoc/>
    public override Double GetDouble(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToDouble(this._data[row]);
    }

    /// <inheritdoc/>
    public override Int16 GetInt16(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToInt16(this._data[row]);
    }

    /// <inheritdoc/>
    public override Int32 GetInt32(Int32 row)
    {
        this.OnGet(row);
        return this._data[row];
    }

    /// <inheritdoc/>
    public override Int64 GetInt64(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToInt64(this._data[row]);
    }

    /// <inheritdoc/>
    public override SByte GetSByte(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToSByte(this._data[row]);
    }

    /// <inheritdoc/>
    public override Single GetSingle(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToSingle(this._data[row]);
    }

    /// <inheritdoc/>
    public override String GetString(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToString(this._data[row]);
    }

    /// <inheritdoc/>
    public override UInt16 GetUInt16(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToUInt16(this._data[row]);
    }

    /// <inheritdoc/>
    public override UInt32 GetUInt32(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToUInt32(this._data[row]);
    }

    /// <inheritdoc/>
    public override UInt64 GetUInt64(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToUInt64(this._data[row]);
    }

    #endregion
}
