using System;

namespace LuYao.Data.Columns;

/// <summary>
/// 表示一个用于存储 <see cref="Single"/> 类型数据的记录列。
/// </summary>
public sealed class SingleRecordColumn : RecordColumn<Single>
{
    internal SingleRecordColumn(Record record, string name)
        : base(record, name, RecordDataType.Single, typeof(Single))
    {
    }

    #region Set

    /// <inheritdoc/>
    public override void Set(Boolean value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Byte value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Char value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(DateTime value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Decimal value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Double value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Int16 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Int32 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Int64 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(SByte value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(Single value, int row)
    {
        this.OnSet(row);
        this._data[row] = value;
    }

    /// <inheritdoc/>
    public override void Set(String value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(UInt16 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(UInt32 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    /// <inheritdoc/>
    public override void Set(UInt64 value, int row)
    {
        this.OnSet(row);
        this._data[row] = Valid.ToSingle(value);
    }

    #endregion

    #region To

    /// <inheritdoc/>
    public override Boolean ToBoolean(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToBoolean(this._data[row]);
    }

    /// <inheritdoc/>
    public override Byte ToByte(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToByte(this._data[row]);
    }

    /// <inheritdoc/>
    public override Char ToChar(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToChar(this._data[row]);
    }

    /// <inheritdoc/>
    public override DateTime ToDateTime(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToDateTime(this._data[row]);
    }

    /// <inheritdoc/>
    public override Decimal ToDecimal(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToDecimal(this._data[row]);
    }

    /// <inheritdoc/>
    public override Double ToDouble(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToDouble(this._data[row]);
    }

    /// <inheritdoc/>
    public override Int16 ToInt16(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToInt16(this._data[row]);
    }

    /// <inheritdoc/>
    public override Int32 ToInt32(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToInt32(this._data[row]);
    }

    /// <inheritdoc/>
    public override Int64 ToInt64(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToInt64(this._data[row]);
    }

    /// <inheritdoc/>
    public override SByte ToSByte(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToSByte(this._data[row]);
    }

    /// <inheritdoc/>
    public override Single ToSingle(Int32 row)
    {
        this.OnGet(row);
        return this._data[row];
    }

    /// <inheritdoc/>
    public override String ToString(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToString(this._data[row]);
    }

    /// <inheritdoc/>
    public override UInt16 ToUInt16(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToUInt16(this._data[row]);
    }

    /// <inheritdoc/>
    public override UInt32 ToUInt32(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToUInt32(this._data[row]);
    }

    /// <inheritdoc/>
    public override UInt64 ToUInt64(Int32 row)
    {
        this.OnGet(row);
        return Valid.ToUInt64(this._data[row]);
    }

    #endregion
}
