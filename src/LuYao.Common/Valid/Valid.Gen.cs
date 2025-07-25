using System;

namespace LuYao
{
    partial class Valid
    {

        #region Boolean
        /// <inheritdoc/>
        public static Boolean ToBoolean(Boolean? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Char? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(SByte? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Byte? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Int16? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(UInt16? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Int32? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(UInt32? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Int64? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(UInt64? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Single? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Double? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Decimal? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(DateTime? value)
        {
            if (value is null) return default;
            return ToBoolean(value.Value);
        }
        /// <inheritdoc/>
        public static Boolean ToBoolean(Boolean value) => value;
        /// <inheritdoc/>
        public static Boolean ToBoolean(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToBoolean(vBoolean);
                case Char vChar: return ToBoolean(vChar);
                case SByte vSByte: return ToBoolean(vSByte);
                case Byte vByte: return ToBoolean(vByte);
                case Int16 vInt16: return ToBoolean(vInt16);
                case UInt16 vUInt16: return ToBoolean(vUInt16);
                case Int32 vInt32: return ToBoolean(vInt32);
                case UInt32 vUInt32: return ToBoolean(vUInt32);
                case Int64 vInt64: return ToBoolean(vInt64);
                case UInt64 vUInt64: return ToBoolean(vUInt64);
                case Single vSingle: return ToBoolean(vSingle);
                case Double vDouble: return ToBoolean(vDouble);
                case Decimal vDecimal: return ToBoolean(vDecimal);
                case DateTime vDateTime: return ToBoolean(vDateTime);
                case String vString: return ToBoolean(vString);
            }
            try { if (value is IConvertible conv) return conv.ToBoolean(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToBoolean(tmp);
        }
        #endregion

        #region Char
        /// <inheritdoc/>
        public static Char ToChar(Boolean? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Char? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(SByte? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Byte? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Int16? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(UInt16? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Int32? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(UInt32? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Int64? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(UInt64? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Single? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Double? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Decimal? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(DateTime? value)
        {
            if (value is null) return default;
            return ToChar(value.Value);
        }
        /// <inheritdoc/>
        public static Char ToChar(String? value)
        {
            if (value is null) return default;
            return Convert.ToChar(value);
        }
        /// <inheritdoc/>
        public static Char ToChar(Boolean value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Char value) => value;
        /// <inheritdoc/>
        public static Char ToChar(SByte value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Byte value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Int16 value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(UInt16 value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Int32 value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(UInt32 value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Int64 value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(UInt64 value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Single value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Double value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Decimal value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(DateTime value) => Convert.ToChar(value);
        /// <inheritdoc/>
        public static Char ToChar(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToChar(vBoolean);
                case Char vChar: return ToChar(vChar);
                case SByte vSByte: return ToChar(vSByte);
                case Byte vByte: return ToChar(vByte);
                case Int16 vInt16: return ToChar(vInt16);
                case UInt16 vUInt16: return ToChar(vUInt16);
                case Int32 vInt32: return ToChar(vInt32);
                case UInt32 vUInt32: return ToChar(vUInt32);
                case Int64 vInt64: return ToChar(vInt64);
                case UInt64 vUInt64: return ToChar(vUInt64);
                case Single vSingle: return ToChar(vSingle);
                case Double vDouble: return ToChar(vDouble);
                case Decimal vDecimal: return ToChar(vDecimal);
                case DateTime vDateTime: return ToChar(vDateTime);
                case String vString: return ToChar(vString);
            }
            try { if (value is IConvertible conv) return conv.ToChar(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToChar(tmp);
        }
        #endregion

        #region SByte
        /// <inheritdoc/>
        public static SByte ToSByte(Boolean? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Char? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(SByte? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Byte? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Int16? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(UInt16? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Int32? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(UInt32? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Int64? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(UInt64? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Single? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Double? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Decimal? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(DateTime? value)
        {
            if (value is null) return default;
            return ToSByte(value.Value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(String? value)
        {
            if (value is null) return default;
            return Convert.ToSByte(value);
        }
        /// <inheritdoc/>
        public static SByte ToSByte(Boolean value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Char value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(SByte value) => value;
        /// <inheritdoc/>
        public static SByte ToSByte(Byte value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Int16 value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(UInt16 value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Int32 value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(UInt32 value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Int64 value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(UInt64 value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Single value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Double value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Decimal value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(DateTime value) => Convert.ToSByte(value);
        /// <inheritdoc/>
        public static SByte ToSByte(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToSByte(vBoolean);
                case Char vChar: return ToSByte(vChar);
                case SByte vSByte: return ToSByte(vSByte);
                case Byte vByte: return ToSByte(vByte);
                case Int16 vInt16: return ToSByte(vInt16);
                case UInt16 vUInt16: return ToSByte(vUInt16);
                case Int32 vInt32: return ToSByte(vInt32);
                case UInt32 vUInt32: return ToSByte(vUInt32);
                case Int64 vInt64: return ToSByte(vInt64);
                case UInt64 vUInt64: return ToSByte(vUInt64);
                case Single vSingle: return ToSByte(vSingle);
                case Double vDouble: return ToSByte(vDouble);
                case Decimal vDecimal: return ToSByte(vDecimal);
                case DateTime vDateTime: return ToSByte(vDateTime);
                case String vString: return ToSByte(vString);
            }
            try { if (value is IConvertible conv) return conv.ToSByte(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToSByte(tmp);
        }
        #endregion

        #region Byte
        /// <inheritdoc/>
        public static Byte ToByte(Boolean? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Char? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(SByte? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Byte? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Int16? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(UInt16? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Int32? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(UInt32? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Int64? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(UInt64? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Single? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Double? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Decimal? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(DateTime? value)
        {
            if (value is null) return default;
            return ToByte(value.Value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(String? value)
        {
            if (value is null) return default;
            return Convert.ToByte(value);
        }
        /// <inheritdoc/>
        public static Byte ToByte(Boolean value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Char value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(SByte value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Byte value) => value;
        /// <inheritdoc/>
        public static Byte ToByte(Int16 value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(UInt16 value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Int32 value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(UInt32 value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Int64 value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(UInt64 value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Single value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Double value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Decimal value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(DateTime value) => Convert.ToByte(value);
        /// <inheritdoc/>
        public static Byte ToByte(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToByte(vBoolean);
                case Char vChar: return ToByte(vChar);
                case SByte vSByte: return ToByte(vSByte);
                case Byte vByte: return ToByte(vByte);
                case Int16 vInt16: return ToByte(vInt16);
                case UInt16 vUInt16: return ToByte(vUInt16);
                case Int32 vInt32: return ToByte(vInt32);
                case UInt32 vUInt32: return ToByte(vUInt32);
                case Int64 vInt64: return ToByte(vInt64);
                case UInt64 vUInt64: return ToByte(vUInt64);
                case Single vSingle: return ToByte(vSingle);
                case Double vDouble: return ToByte(vDouble);
                case Decimal vDecimal: return ToByte(vDecimal);
                case DateTime vDateTime: return ToByte(vDateTime);
                case String vString: return ToByte(vString);
            }
            try { if (value is IConvertible conv) return conv.ToByte(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToByte(tmp);
        }
        #endregion

        #region Int16
        /// <inheritdoc/>
        public static Int16 ToInt16(Boolean? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Char? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(SByte? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Byte? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Int16? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(UInt16? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Int32? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(UInt32? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Int64? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(UInt64? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Single? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Double? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Decimal? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(DateTime? value)
        {
            if (value is null) return default;
            return ToInt16(value.Value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(String? value)
        {
            if (value is null) return default;
            return Convert.ToInt16(value);
        }
        /// <inheritdoc/>
        public static Int16 ToInt16(Boolean value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Char value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(SByte value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Byte value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Int16 value) => value;
        /// <inheritdoc/>
        public static Int16 ToInt16(UInt16 value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Int32 value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(UInt32 value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Int64 value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(UInt64 value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Single value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Double value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Decimal value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(DateTime value) => Convert.ToInt16(value);
        /// <inheritdoc/>
        public static Int16 ToInt16(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToInt16(vBoolean);
                case Char vChar: return ToInt16(vChar);
                case SByte vSByte: return ToInt16(vSByte);
                case Byte vByte: return ToInt16(vByte);
                case Int16 vInt16: return ToInt16(vInt16);
                case UInt16 vUInt16: return ToInt16(vUInt16);
                case Int32 vInt32: return ToInt16(vInt32);
                case UInt32 vUInt32: return ToInt16(vUInt32);
                case Int64 vInt64: return ToInt16(vInt64);
                case UInt64 vUInt64: return ToInt16(vUInt64);
                case Single vSingle: return ToInt16(vSingle);
                case Double vDouble: return ToInt16(vDouble);
                case Decimal vDecimal: return ToInt16(vDecimal);
                case DateTime vDateTime: return ToInt16(vDateTime);
                case String vString: return ToInt16(vString);
            }
            try { if (value is IConvertible conv) return conv.ToInt16(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToInt16(tmp);
        }
        #endregion

        #region UInt16
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Boolean? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Char? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(SByte? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Byte? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Int16? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(UInt16? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Int32? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(UInt32? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Int64? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(UInt64? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Single? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Double? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Decimal? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(DateTime? value)
        {
            if (value is null) return default;
            return ToUInt16(value.Value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(String? value)
        {
            if (value is null) return default;
            return Convert.ToUInt16(value);
        }
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Boolean value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Char value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(SByte value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Byte value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Int16 value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(UInt16 value) => value;
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Int32 value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(UInt32 value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Int64 value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(UInt64 value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Single value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Double value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Decimal value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(DateTime value) => Convert.ToUInt16(value);
        /// <inheritdoc/>
        public static UInt16 ToUInt16(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToUInt16(vBoolean);
                case Char vChar: return ToUInt16(vChar);
                case SByte vSByte: return ToUInt16(vSByte);
                case Byte vByte: return ToUInt16(vByte);
                case Int16 vInt16: return ToUInt16(vInt16);
                case UInt16 vUInt16: return ToUInt16(vUInt16);
                case Int32 vInt32: return ToUInt16(vInt32);
                case UInt32 vUInt32: return ToUInt16(vUInt32);
                case Int64 vInt64: return ToUInt16(vInt64);
                case UInt64 vUInt64: return ToUInt16(vUInt64);
                case Single vSingle: return ToUInt16(vSingle);
                case Double vDouble: return ToUInt16(vDouble);
                case Decimal vDecimal: return ToUInt16(vDecimal);
                case DateTime vDateTime: return ToUInt16(vDateTime);
                case String vString: return ToUInt16(vString);
            }
            try { if (value is IConvertible conv) return conv.ToUInt16(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToUInt16(tmp);
        }
        #endregion

        #region Int32
        /// <inheritdoc/>
        public static Int32 ToInt32(Boolean? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Char? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(SByte? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Byte? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Int16? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(UInt16? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Int32? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(UInt32? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Int64? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(UInt64? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Single? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Double? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Decimal? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(DateTime? value)
        {
            if (value is null) return default;
            return ToInt32(value.Value);
        }
        /// <inheritdoc/>
        public static Int32 ToInt32(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToInt32(vBoolean);
                case Char vChar: return ToInt32(vChar);
                case SByte vSByte: return ToInt32(vSByte);
                case Byte vByte: return ToInt32(vByte);
                case Int16 vInt16: return ToInt32(vInt16);
                case UInt16 vUInt16: return ToInt32(vUInt16);
                case Int32 vInt32: return ToInt32(vInt32);
                case UInt32 vUInt32: return ToInt32(vUInt32);
                case Int64 vInt64: return ToInt32(vInt64);
                case UInt64 vUInt64: return ToInt32(vUInt64);
                case Single vSingle: return ToInt32(vSingle);
                case Double vDouble: return ToInt32(vDouble);
                case Decimal vDecimal: return ToInt32(vDecimal);
                case DateTime vDateTime: return ToInt32(vDateTime);
                case String vString: return ToInt32(vString);
            }
            try { if (value is IConvertible conv) return conv.ToInt32(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToInt32(tmp);
        }
        #endregion

        #region UInt32
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Boolean? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Char? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(SByte? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Byte? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Int16? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(UInt16? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Int32? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(UInt32? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Int64? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(UInt64? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Single? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Double? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Decimal? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(DateTime? value)
        {
            if (value is null) return default;
            return ToUInt32(value.Value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(String? value)
        {
            if (value is null) return default;
            return Convert.ToUInt32(value);
        }
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Boolean value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Char value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(SByte value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Byte value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Int16 value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(UInt16 value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Int32 value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(UInt32 value) => value;
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Int64 value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(UInt64 value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Single value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Double value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Decimal value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(DateTime value) => Convert.ToUInt32(value);
        /// <inheritdoc/>
        public static UInt32 ToUInt32(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToUInt32(vBoolean);
                case Char vChar: return ToUInt32(vChar);
                case SByte vSByte: return ToUInt32(vSByte);
                case Byte vByte: return ToUInt32(vByte);
                case Int16 vInt16: return ToUInt32(vInt16);
                case UInt16 vUInt16: return ToUInt32(vUInt16);
                case Int32 vInt32: return ToUInt32(vInt32);
                case UInt32 vUInt32: return ToUInt32(vUInt32);
                case Int64 vInt64: return ToUInt32(vInt64);
                case UInt64 vUInt64: return ToUInt32(vUInt64);
                case Single vSingle: return ToUInt32(vSingle);
                case Double vDouble: return ToUInt32(vDouble);
                case Decimal vDecimal: return ToUInt32(vDecimal);
                case DateTime vDateTime: return ToUInt32(vDateTime);
                case String vString: return ToUInt32(vString);
            }
            try { if (value is IConvertible conv) return conv.ToUInt32(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToUInt32(tmp);
        }
        #endregion

        #region Int64
        /// <inheritdoc/>
        public static Int64 ToInt64(Boolean? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Char? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(SByte? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Byte? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Int16? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(UInt16? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Int32? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(UInt32? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Int64? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(UInt64? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Single? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Double? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Decimal? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(DateTime? value)
        {
            if (value is null) return default;
            return ToInt64(value.Value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(String? value)
        {
            if (value is null) return default;
            return Convert.ToInt64(value);
        }
        /// <inheritdoc/>
        public static Int64 ToInt64(Boolean value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Char value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(SByte value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Byte value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Int16 value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(UInt16 value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Int32 value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(UInt32 value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Int64 value) => value;
        /// <inheritdoc/>
        public static Int64 ToInt64(UInt64 value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Single value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Double value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Decimal value) => Convert.ToInt64(value);
        /// <inheritdoc/>
        public static Int64 ToInt64(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToInt64(vBoolean);
                case Char vChar: return ToInt64(vChar);
                case SByte vSByte: return ToInt64(vSByte);
                case Byte vByte: return ToInt64(vByte);
                case Int16 vInt16: return ToInt64(vInt16);
                case UInt16 vUInt16: return ToInt64(vUInt16);
                case Int32 vInt32: return ToInt64(vInt32);
                case UInt32 vUInt32: return ToInt64(vUInt32);
                case Int64 vInt64: return ToInt64(vInt64);
                case UInt64 vUInt64: return ToInt64(vUInt64);
                case Single vSingle: return ToInt64(vSingle);
                case Double vDouble: return ToInt64(vDouble);
                case Decimal vDecimal: return ToInt64(vDecimal);
                case DateTime vDateTime: return ToInt64(vDateTime);
                case String vString: return ToInt64(vString);
            }
            try { if (value is IConvertible conv) return conv.ToInt64(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToInt64(tmp);
        }
        #endregion

        #region UInt64
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Boolean? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Char? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(SByte? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Byte? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Int16? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(UInt16? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Int32? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(UInt32? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Int64? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(UInt64? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Single? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Double? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Decimal? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(DateTime? value)
        {
            if (value is null) return default;
            return ToUInt64(value.Value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(String? value)
        {
            if (value is null) return default;
            return Convert.ToUInt64(value);
        }
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Boolean value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Char value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(SByte value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Byte value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Int16 value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(UInt16 value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Int32 value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(UInt32 value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Int64 value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(UInt64 value) => value;
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Single value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Double value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Decimal value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(DateTime value) => Convert.ToUInt64(value);
        /// <inheritdoc/>
        public static UInt64 ToUInt64(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToUInt64(vBoolean);
                case Char vChar: return ToUInt64(vChar);
                case SByte vSByte: return ToUInt64(vSByte);
                case Byte vByte: return ToUInt64(vByte);
                case Int16 vInt16: return ToUInt64(vInt16);
                case UInt16 vUInt16: return ToUInt64(vUInt16);
                case Int32 vInt32: return ToUInt64(vInt32);
                case UInt32 vUInt32: return ToUInt64(vUInt32);
                case Int64 vInt64: return ToUInt64(vInt64);
                case UInt64 vUInt64: return ToUInt64(vUInt64);
                case Single vSingle: return ToUInt64(vSingle);
                case Double vDouble: return ToUInt64(vDouble);
                case Decimal vDecimal: return ToUInt64(vDecimal);
                case DateTime vDateTime: return ToUInt64(vDateTime);
                case String vString: return ToUInt64(vString);
            }
            try { if (value is IConvertible conv) return conv.ToUInt64(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToUInt64(tmp);
        }
        #endregion

        #region Single
        /// <inheritdoc/>
        public static Single ToSingle(Boolean? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Char? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(SByte? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Byte? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Int16? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(UInt16? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Int32? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(UInt32? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Int64? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(UInt64? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Single? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Double? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Decimal? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(DateTime? value)
        {
            if (value is null) return default;
            return ToSingle(value.Value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(String? value)
        {
            if (value is null) return default;
            return Convert.ToSingle(value);
        }
        /// <inheritdoc/>
        public static Single ToSingle(Boolean value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Char value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(SByte value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Byte value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Int16 value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(UInt16 value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Int32 value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(UInt32 value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Int64 value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(UInt64 value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Single value) => value;
        /// <inheritdoc/>
        public static Single ToSingle(Double value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Decimal value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(DateTime value) => Convert.ToSingle(value);
        /// <inheritdoc/>
        public static Single ToSingle(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToSingle(vBoolean);
                case Char vChar: return ToSingle(vChar);
                case SByte vSByte: return ToSingle(vSByte);
                case Byte vByte: return ToSingle(vByte);
                case Int16 vInt16: return ToSingle(vInt16);
                case UInt16 vUInt16: return ToSingle(vUInt16);
                case Int32 vInt32: return ToSingle(vInt32);
                case UInt32 vUInt32: return ToSingle(vUInt32);
                case Int64 vInt64: return ToSingle(vInt64);
                case UInt64 vUInt64: return ToSingle(vUInt64);
                case Single vSingle: return ToSingle(vSingle);
                case Double vDouble: return ToSingle(vDouble);
                case Decimal vDecimal: return ToSingle(vDecimal);
                case DateTime vDateTime: return ToSingle(vDateTime);
                case String vString: return ToSingle(vString);
            }
            try { if (value is IConvertible conv) return conv.ToSingle(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToSingle(tmp);
        }
        #endregion

        #region Double
        /// <inheritdoc/>
        public static Double ToDouble(Boolean? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Char? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(SByte? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Byte? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Int16? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(UInt16? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Int32? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(UInt32? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Int64? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(UInt64? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Single? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Double? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Decimal? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(DateTime? value)
        {
            if (value is null) return default;
            return ToDouble(value.Value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(String? value)
        {
            if (value is null) return default;
            return Convert.ToDouble(value);
        }
        /// <inheritdoc/>
        public static Double ToDouble(Boolean value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Char value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(SByte value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Byte value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Int16 value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(UInt16 value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Int32 value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(UInt32 value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Int64 value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(UInt64 value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Single value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Double value) => value;
        /// <inheritdoc/>
        public static Double ToDouble(Decimal value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(DateTime value) => Convert.ToDouble(value);
        /// <inheritdoc/>
        public static Double ToDouble(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToDouble(vBoolean);
                case Char vChar: return ToDouble(vChar);
                case SByte vSByte: return ToDouble(vSByte);
                case Byte vByte: return ToDouble(vByte);
                case Int16 vInt16: return ToDouble(vInt16);
                case UInt16 vUInt16: return ToDouble(vUInt16);
                case Int32 vInt32: return ToDouble(vInt32);
                case UInt32 vUInt32: return ToDouble(vUInt32);
                case Int64 vInt64: return ToDouble(vInt64);
                case UInt64 vUInt64: return ToDouble(vUInt64);
                case Single vSingle: return ToDouble(vSingle);
                case Double vDouble: return ToDouble(vDouble);
                case Decimal vDecimal: return ToDouble(vDecimal);
                case DateTime vDateTime: return ToDouble(vDateTime);
                case String vString: return ToDouble(vString);
            }
            try { if (value is IConvertible conv) return conv.ToDouble(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToDouble(tmp);
        }
        #endregion

        #region Decimal
        /// <inheritdoc/>
        public static Decimal ToDecimal(Boolean? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Char? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(SByte? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Byte? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Int16? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(UInt16? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Int32? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(UInt32? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Int64? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(UInt64? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Single? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Double? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Decimal? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(DateTime? value)
        {
            if (value is null) return default;
            return ToDecimal(value.Value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(String? value)
        {
            if (value is null) return default;
            return Convert.ToDecimal(value);
        }
        /// <inheritdoc/>
        public static Decimal ToDecimal(Boolean value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Char value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(SByte value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Byte value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Int16 value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(UInt16 value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Int32 value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(UInt32 value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Int64 value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(UInt64 value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Single value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Double value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Decimal value) => value;
        /// <inheritdoc/>
        public static Decimal ToDecimal(DateTime value) => Convert.ToDecimal(value);
        /// <inheritdoc/>
        public static Decimal ToDecimal(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToDecimal(vBoolean);
                case Char vChar: return ToDecimal(vChar);
                case SByte vSByte: return ToDecimal(vSByte);
                case Byte vByte: return ToDecimal(vByte);
                case Int16 vInt16: return ToDecimal(vInt16);
                case UInt16 vUInt16: return ToDecimal(vUInt16);
                case Int32 vInt32: return ToDecimal(vInt32);
                case UInt32 vUInt32: return ToDecimal(vUInt32);
                case Int64 vInt64: return ToDecimal(vInt64);
                case UInt64 vUInt64: return ToDecimal(vUInt64);
                case Single vSingle: return ToDecimal(vSingle);
                case Double vDouble: return ToDecimal(vDouble);
                case Decimal vDecimal: return ToDecimal(vDecimal);
                case DateTime vDateTime: return ToDecimal(vDateTime);
                case String vString: return ToDecimal(vString);
            }
            try { if (value is IConvertible conv) return conv.ToDecimal(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToDecimal(tmp);
        }
        #endregion

        #region DateTime
        /// <inheritdoc/>
        public static DateTime ToDateTime(Boolean? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Char? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(SByte? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Byte? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Int16? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(UInt16? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Int32? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(UInt32? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Int64? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(UInt64? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Single? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Double? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Decimal? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(DateTime? value)
        {
            if (value is null) return default;
            return ToDateTime(value.Value);
        }
        /// <inheritdoc/>
        public static DateTime ToDateTime(Boolean value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Char value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(SByte value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Byte value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Int16 value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(UInt16 value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Int32 value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(UInt32 value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(UInt64 value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Single value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Double value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(Decimal value) => Convert.ToDateTime(value);
        /// <inheritdoc/>
        public static DateTime ToDateTime(DateTime value) => value;
        /// <inheritdoc/>
        public static DateTime ToDateTime(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return default;
            switch (value)
            {
                case Boolean vBoolean: return ToDateTime(vBoolean);
                case Char vChar: return ToDateTime(vChar);
                case SByte vSByte: return ToDateTime(vSByte);
                case Byte vByte: return ToDateTime(vByte);
                case Int16 vInt16: return ToDateTime(vInt16);
                case UInt16 vUInt16: return ToDateTime(vUInt16);
                case Int32 vInt32: return ToDateTime(vInt32);
                case UInt32 vUInt32: return ToDateTime(vUInt32);
                case Int64 vInt64: return ToDateTime(vInt64);
                case UInt64 vUInt64: return ToDateTime(vUInt64);
                case Single vSingle: return ToDateTime(vSingle);
                case Double vDouble: return ToDateTime(vDouble);
                case Decimal vDecimal: return ToDateTime(vDecimal);
                case DateTime vDateTime: return ToDateTime(vDateTime);
                case String vString: return ToDateTime(vString);
            }
            try { if (value is IConvertible conv) return conv.ToDateTime(null); }
            catch { }
            string? tmp = value.ToString();
            if (string.IsNullOrWhiteSpace(tmp)) return default;
            return ToDateTime(tmp);
        }
        #endregion

        #region String
        /// <inheritdoc/>
        public static String ToString(Boolean? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Char? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(SByte? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Byte? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Int16? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(UInt16? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Int32? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(UInt32? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Int64? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(UInt64? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Single? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Double? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(Decimal? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(DateTime? value)
        {
            if (value is null) return String.Empty;
            return ToString(value.Value);
        }
        /// <inheritdoc/>
        public static String ToString(String? value)
        {
            if (value is null) return String.Empty;
            return Convert.ToString(value);
        }
        /// <inheritdoc/>
        public static String ToString(Object? value)
        {
            if (value is null || Convert.IsDBNull(value)) return String.Empty;
            switch (value)
            {
                case Boolean vBoolean: return ToString(vBoolean);
                case Char vChar: return ToString(vChar);
                case SByte vSByte: return ToString(vSByte);
                case Byte vByte: return ToString(vByte);
                case Int16 vInt16: return ToString(vInt16);
                case UInt16 vUInt16: return ToString(vUInt16);
                case Int32 vInt32: return ToString(vInt32);
                case UInt32 vUInt32: return ToString(vUInt32);
                case Int64 vInt64: return ToString(vInt64);
                case UInt64 vUInt64: return ToString(vUInt64);
                case Single vSingle: return ToString(vSingle);
                case Double vDouble: return ToString(vDouble);
                case Decimal vDecimal: return ToString(vDecimal);
                case DateTime vDateTime: return ToString(vDateTime);
                case String vString: return ToString(vString);
            }
            return Convert.ToString(value) ?? String.Empty;
        }
        #endregion
    }
}
