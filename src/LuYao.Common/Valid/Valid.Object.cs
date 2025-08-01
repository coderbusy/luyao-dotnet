using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao;

partial class Valid
{
    /// <summary>
    /// 将指定的值转换为指定的类型。
    /// </summary>
    /// <param name="value">要转换的值。</param>
    /// <param name="type">目标类型。</param>
    /// <returns>转换后的对象。</returns>
    public static object To(Object value, Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        switch (Type.GetTypeCode(underlyingType))
        {
            case TypeCode.Boolean: return Valid.ToBoolean(value);
            case TypeCode.Byte: return Valid.ToByte(value);
            case TypeCode.Char: return Valid.ToChar(value);
            case TypeCode.DateTime: return Valid.ToDateTime(value);
            case TypeCode.Decimal: return Valid.ToDecimal(value);
            case TypeCode.Double: return Valid.ToDouble(value);
            case TypeCode.Int16: return Valid.ToInt16(value);
            case TypeCode.Int32: return Valid.ToInt32(value);
            case TypeCode.Int64: return Valid.ToInt64(value);
            case TypeCode.SByte: return Valid.ToSByte(value);
            case TypeCode.Single: return Valid.ToSingle(value);
            case TypeCode.String: return Valid.ToString(value);
            case TypeCode.UInt16: return Valid.ToUInt16(value);
            case TypeCode.UInt32: return Valid.ToUInt32(value);
            case TypeCode.UInt64: return Valid.ToUInt64(value);
        }
        return Convert.ChangeType(value, underlyingType);
    }
}
