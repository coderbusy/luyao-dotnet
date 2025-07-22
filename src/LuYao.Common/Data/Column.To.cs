using System;

namespace LuYao.Data;

partial class Column
{
    ///<inheritdoc/>
    public override string ToString()
    {
        return string.Empty;
    }

    /// <inheritdoc/>
    public Boolean ToBoolean(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return this.Data.GetValue<Boolean>(index);
            case TypeCode.Byte: return Valid.ToBoolean(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToBoolean(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToBoolean(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToBoolean(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToBoolean(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToBoolean(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToBoolean(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToBoolean(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToBoolean(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToBoolean(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToBoolean(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToBoolean(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToBoolean(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToBoolean(this.Data.GetValue<UInt64>(index));
            default: return default(Boolean);
        }
    }

    /// <inheritdoc/>
    public Byte ToByte(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToByte(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return this.Data.GetValue<Byte>(index);
            case TypeCode.Char: return Valid.ToByte(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToByte(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToByte(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToByte(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToByte(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToByte(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToByte(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToByte(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToByte(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToByte(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToByte(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToByte(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToByte(this.Data.GetValue<UInt64>(index));
            default: return default(Byte);
        }
    }

    /// <inheritdoc/>
    public Char ToChar(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToChar(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToChar(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return this.Data.GetValue<Char>(index);
            case TypeCode.DateTime: return Valid.ToChar(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToChar(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToChar(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToChar(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToChar(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToChar(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToChar(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToChar(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToChar(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToChar(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToChar(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToChar(this.Data.GetValue<UInt64>(index));
            default: return default(Char);
        }
    }

    /// <inheritdoc/>
    public DateTime ToDateTime(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToDateTime(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToDateTime(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToDateTime(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return this.Data.GetValue<DateTime>(index);
            case TypeCode.Decimal: return Valid.ToDateTime(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToDateTime(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToDateTime(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToDateTime(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToDateTime(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToDateTime(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToDateTime(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToDateTime(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToDateTime(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToDateTime(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToDateTime(this.Data.GetValue<UInt64>(index));
            default: return default(DateTime);
        }
    }

    /// <inheritdoc/>
    public Decimal ToDecimal(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToDecimal(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToDecimal(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToDecimal(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToDecimal(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return this.Data.GetValue<Decimal>(index);
            case TypeCode.Double: return Valid.ToDecimal(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToDecimal(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToDecimal(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToDecimal(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToDecimal(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToDecimal(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToDecimal(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToDecimal(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToDecimal(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToDecimal(this.Data.GetValue<UInt64>(index));
            default: return default(Decimal);
        }
    }

    /// <inheritdoc/>
    public Double ToDouble(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToDouble(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToDouble(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToDouble(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToDouble(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToDouble(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return this.Data.GetValue<Double>(index);
            case TypeCode.Int16: return Valid.ToDouble(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToDouble(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToDouble(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToDouble(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToDouble(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToDouble(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToDouble(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToDouble(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToDouble(this.Data.GetValue<UInt64>(index));
            default: return default(Double);
        }
    }

    /// <inheritdoc/>
    public Int16 ToInt16(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToInt16(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToInt16(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToInt16(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToInt16(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToInt16(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToInt16(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return this.Data.GetValue<Int16>(index);
            case TypeCode.Int32: return Valid.ToInt16(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToInt16(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToInt16(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToInt16(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToInt16(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToInt16(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToInt16(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToInt16(this.Data.GetValue<UInt64>(index));
            default: return default(Int16);
        }
    }

    /// <inheritdoc/>
    public Int32 ToInt32(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToInt32(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToInt32(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToInt32(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToInt32(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToInt32(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToInt32(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToInt32(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return this.Data.GetValue<Int32>(index);
            case TypeCode.Int64: return Valid.ToInt32(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToInt32(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToInt32(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToInt32(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToInt32(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToInt32(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToInt32(this.Data.GetValue<UInt64>(index));
            default: return default(Int32);
        }
    }

    /// <inheritdoc/>
    public Int64 ToInt64(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToInt64(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToInt64(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToInt64(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToInt64(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToInt64(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToInt64(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToInt64(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToInt64(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return this.Data.GetValue<Int64>(index);
            case TypeCode.SByte: return Valid.ToInt64(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToInt64(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToInt64(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToInt64(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToInt64(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToInt64(this.Data.GetValue<UInt64>(index));
            default: return default(Int64);
        }
    }

    /// <inheritdoc/>
    public SByte ToSByte(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToSByte(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToSByte(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToSByte(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToSByte(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToSByte(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToSByte(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToSByte(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToSByte(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToSByte(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return this.Data.GetValue<SByte>(index);
            case TypeCode.Single: return Valid.ToSByte(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToSByte(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToSByte(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToSByte(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToSByte(this.Data.GetValue<UInt64>(index));
            default: return default(SByte);
        }
    }

    /// <inheritdoc/>
    public Single ToSingle(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToSingle(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToSingle(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToSingle(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToSingle(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToSingle(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToSingle(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToSingle(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToSingle(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToSingle(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToSingle(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return this.Data.GetValue<Single>(index);
            case TypeCode.String: return Valid.ToSingle(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToSingle(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToSingle(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToSingle(this.Data.GetValue<UInt64>(index));
            default: return default(Single);
        }
    }

    /// <inheritdoc/>
    public String ToString(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToString(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToString(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToString(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToString(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToString(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToString(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToString(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToString(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToString(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToString(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToString(this.Data.GetValue<Single>(index));
            case TypeCode.String: return this.Data.GetValue<String>(index);
            case TypeCode.UInt16: return Valid.ToString(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToString(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToString(this.Data.GetValue<UInt64>(index));
            default: return string.Empty;
        }
    }

    /// <inheritdoc/>
    public UInt16 ToUInt16(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToUInt16(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToUInt16(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToUInt16(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToUInt16(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToUInt16(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToUInt16(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToUInt16(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToUInt16(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToUInt16(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToUInt16(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToUInt16(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToUInt16(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return this.Data.GetValue<UInt16>(index);
            case TypeCode.UInt32: return Valid.ToUInt16(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return Valid.ToUInt16(this.Data.GetValue<UInt64>(index));
            default: return default(UInt16);
        }
    }

    /// <inheritdoc/>
    public UInt32 ToUInt32(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToUInt32(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToUInt32(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToUInt32(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToUInt32(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToUInt32(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToUInt32(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToUInt32(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToUInt32(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToUInt32(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToUInt32(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToUInt32(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToUInt32(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToUInt32(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return this.Data.GetValue<UInt32>(index);
            case TypeCode.UInt64: return Valid.ToUInt32(this.Data.GetValue<UInt64>(index));
            default: return default(UInt32);
        }
    }

    /// <inheritdoc/>
    public UInt64 ToUInt64(int index)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToUInt64(this.Data.GetValue<Boolean>(index));
            case TypeCode.Byte: return Valid.ToUInt64(this.Data.GetValue<Byte>(index));
            case TypeCode.Char: return Valid.ToUInt64(this.Data.GetValue<Char>(index));
            case TypeCode.DateTime: return Valid.ToUInt64(this.Data.GetValue<DateTime>(index));
            case TypeCode.Decimal: return Valid.ToUInt64(this.Data.GetValue<Decimal>(index));
            case TypeCode.Double: return Valid.ToUInt64(this.Data.GetValue<Double>(index));
            case TypeCode.Int16: return Valid.ToUInt64(this.Data.GetValue<Int16>(index));
            case TypeCode.Int32: return Valid.ToUInt64(this.Data.GetValue<Int32>(index));
            case TypeCode.Int64: return Valid.ToUInt64(this.Data.GetValue<Int64>(index));
            case TypeCode.SByte: return Valid.ToUInt64(this.Data.GetValue<SByte>(index));
            case TypeCode.Single: return Valid.ToUInt64(this.Data.GetValue<Single>(index));
            case TypeCode.String: return Valid.ToUInt64(this.Data.GetValue<String>(index));
            case TypeCode.UInt16: return Valid.ToUInt64(this.Data.GetValue<UInt16>(index));
            case TypeCode.UInt32: return Valid.ToUInt64(this.Data.GetValue<UInt32>(index));
            case TypeCode.UInt64: return this.Data.GetValue<UInt64>(index);
            default: return default(UInt64);
        }
    }


}
