using System;

namespace LuYao;

partial class TypeConvert
{
    /// <summary>
    /// Converts the specified value to the specified type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="type">The target type.</param>
    /// <returns>The converted object.</returns>
    public static object ChangeType(Object value, Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        switch (Type.GetTypeCode(underlyingType))
        {
            case TypeCode.Boolean: return TypeConvert.ToBoolean(value);
            case TypeCode.Byte: return TypeConvert.ToByte(value);
            case TypeCode.Char: return TypeConvert.ToChar(value);
            case TypeCode.DateTime: return TypeConvert.ToDateTime(value);
            case TypeCode.Decimal: return TypeConvert.ToDecimal(value);
            case TypeCode.Double: return TypeConvert.ToDouble(value);
            case TypeCode.Int16: return TypeConvert.ToInt16(value);
            case TypeCode.Int32: return TypeConvert.ToInt32(value);
            case TypeCode.Int64: return TypeConvert.ToInt64(value);
            case TypeCode.SByte: return TypeConvert.ToSByte(value);
            case TypeCode.Single: return TypeConvert.ToSingle(value);
            case TypeCode.String: return TypeConvert.ToString(value);
            case TypeCode.UInt16: return TypeConvert.ToUInt16(value);
            case TypeCode.UInt32: return TypeConvert.ToUInt32(value);
            case TypeCode.UInt64: return TypeConvert.ToUInt64(value);
        }
        return Convert.ChangeType(value, underlyingType);
    }
}
