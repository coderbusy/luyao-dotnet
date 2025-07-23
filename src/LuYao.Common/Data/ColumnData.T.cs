using System;

namespace LuYao.Data;

#region Boolean
sealed class BooleanColumnData : ColumnData<Boolean>
{
    public BooleanColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = value;
    public override Boolean ToBoolean(int index) => this._data[index];

    public override void Set(Byte value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToBoolean(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToBoolean(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToBoolean(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToBoolean(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToBoolean(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToBoolean(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToBoolean(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Byte
sealed class ByteColumnData : ColumnData<Byte>
{
    public ByteColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToByte(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = value;
    public override Byte ToByte(int index) => this._data[index];

    public override void Set(Char value, int index) => _data[index] = Valid.ToByte(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToByte(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToByte(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToByte(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToByte(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToByte(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToByte(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToByte(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToByte(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToByte(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToByte(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToByte(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToByte(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Char
sealed class CharColumnData : ColumnData<Char>
{
    public CharColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToChar(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToChar(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = value;
    public override Char ToChar(int index) => this._data[index];

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToChar(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToChar(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToChar(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToChar(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToChar(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToChar(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToChar(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToChar(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToChar(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToChar(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToChar(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToChar(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region DateTime
sealed class DateTimeColumnData : ColumnData<DateTime>
{
    public DateTimeColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = value;
    public override DateTime ToDateTime(int index) => this._data[index];

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToDateTime(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToDateTime(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToDateTime(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToDateTime(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToDateTime(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToDateTime(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Decimal
sealed class DecimalColumnData : ColumnData<Decimal>
{
    public DecimalColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToDecimal(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = value;
    public override Decimal ToDecimal(int index) => this._data[index];

    public override void Set(Double value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToDecimal(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToDecimal(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToDecimal(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToDecimal(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToDecimal(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToDecimal(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Double
sealed class DoubleColumnData : ColumnData<Double>
{
    public DoubleColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToDouble(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToDouble(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToDouble(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToDouble(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToDouble(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = value;
    public override Double ToDouble(int index) => this._data[index];

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToDouble(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToDouble(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToDouble(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToDouble(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToDouble(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToDouble(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToDouble(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToDouble(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToDouble(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Int16
sealed class Int16ColumnData : ColumnData<Int16>
{
    public Int16ColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToInt16(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToInt16(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToInt16(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToInt16(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToInt16(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToInt16(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = value;
    public override Int16 ToInt16(int index) => this._data[index];

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToInt16(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToInt16(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToInt16(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToInt16(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToInt16(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToInt16(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToInt16(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToInt16(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Int32
sealed class Int32ColumnData : ColumnData<Int32>
{
    public Int32ColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToInt32(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToInt32(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToInt32(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToInt32(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToInt32(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToInt32(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToInt32(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = value;
    public override Int32 ToInt32(int index) => this._data[index];

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToInt32(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToInt32(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToInt32(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToInt32(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToInt32(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToInt32(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToInt32(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Int64
sealed class Int64ColumnData : ColumnData<Int64>
{
    public Int64ColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToInt64(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToInt64(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToInt64(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToInt64(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToInt64(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToInt64(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToInt64(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToInt64(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = value;
    public override Int64 ToInt64(int index) => this._data[index];

    public override void Set(SByte value, int index) => _data[index] = Valid.ToInt64(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToInt64(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToInt64(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToInt64(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToInt64(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToInt64(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region SByte
sealed class SByteColumnData : ColumnData<SByte>
{
    public SByteColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToSByte(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToSByte(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToSByte(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToSByte(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToSByte(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToSByte(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToSByte(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToSByte(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToSByte(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = value;
    public override SByte ToSByte(int index) => this._data[index];

    public override void Set(Single value, int index) => _data[index] = Valid.ToSByte(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToSByte(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToSByte(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToSByte(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToSByte(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region Single
sealed class SingleColumnData : ColumnData<Single>
{
    public SingleColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToSingle(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToSingle(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToSingle(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToSingle(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToSingle(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToSingle(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToSingle(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToSingle(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToSingle(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToSingle(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = value;
    public override Single ToSingle(int index) => this._data[index];

    public override void Set(String value, int index) => _data[index] = Valid.ToSingle(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToSingle(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToSingle(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToSingle(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region String
sealed class StringColumnData : ColumnData<String>
{
    public StringColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToString(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToString(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToString(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToString(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToString(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToString(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToString(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToString(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToString(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToString(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToString(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = value;
    public override String ToString(int index) => this._data[index];

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToString(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToString(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToString(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region UInt16
sealed class UInt16ColumnData : ColumnData<UInt16>
{
    public UInt16ColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToUInt16(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToUInt16(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToUInt16(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToUInt16(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = value;
    public override UInt16 ToUInt16(int index) => this._data[index];

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToUInt16(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToUInt16(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region UInt32
sealed class UInt32ColumnData : ColumnData<UInt32>
{
    public UInt32ColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToUInt32(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToUInt32(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToUInt32(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToUInt32(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToUInt32(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = value;
    public override UInt32 ToUInt32(int index) => this._data[index];

    public override void Set(UInt64 value, int index) => _data[index] = Valid.ToUInt32(value);
    public override UInt64 ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}
#endregion

#region UInt64
sealed class UInt64ColumnData : ColumnData<UInt64>
{
    public UInt64ColumnData(int capacity) : base(capacity) { }

    public override void Set(Boolean value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Boolean ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override void Set(Byte value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override void Set(Char value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override void Set(DateTime value, int index) => _data[index] = Valid.ToUInt64(value);
    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override void Set(Decimal value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override void Set(Double value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override void Set(Int16 value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Int16 ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override void Set(Int32 value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Int32 ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override void Set(Int64 value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Int64 ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override void Set(SByte value, int index) => _data[index] = Valid.ToUInt64(value);
    public override SByte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override void Set(Single value, int index) => _data[index] = Valid.ToUInt64(value);
    public override Single ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override void Set(String value, int index) => _data[index] = Valid.ToUInt64(value);
    public override String ToString(int index) => Valid.ToString(this._data[index]);

    public override void Set(UInt16 value, int index) => _data[index] = Valid.ToUInt64(value);
    public override UInt16 ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override void Set(UInt32 value, int index) => _data[index] = Valid.ToUInt64(value);
    public override UInt32 ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override void Set(UInt64 value, int index) => _data[index] = value;
    public override UInt64 ToUInt64(int index) => this._data[index];
}
#endregion

