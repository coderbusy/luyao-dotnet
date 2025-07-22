using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

partial class Column
{
    /// <inheritdoc/>
    public void Set(Boolean value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Byte value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Char value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(DateTime value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Decimal value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Double value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Int16 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Int32 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Int64 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(SByte value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(Single value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(String value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(UInt16 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(UInt32 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

    /// <inheritdoc/>
    public void Set(UInt64 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Boolean: this.Data.SetValue(Valid.ToBoolean(value), index); break;
            case TypeCode.Byte: this.Data.SetValue(Valid.ToByte(value), index); break;
            case TypeCode.Char: this.Data.SetValue(Valid.ToChar(value), index); break;
            case TypeCode.DateTime: this.Data.SetValue(Valid.ToDateTime(value), index); break;
            case TypeCode.Decimal: this.Data.SetValue(Valid.ToDecimal(value), index); break;
            case TypeCode.Double: this.Data.SetValue(Valid.ToDouble(value), index); break;
            case TypeCode.Int16: this.Data.SetValue(Valid.ToInt16(value), index); break;
            case TypeCode.Int32: this.Data.SetValue(Valid.ToInt32(value), index); break;
            case TypeCode.Int64: this.Data.SetValue(Valid.ToInt64(value), index); break;
            case TypeCode.SByte: this.Data.SetValue(Valid.ToSByte(value), index); break;
            case TypeCode.Single: this.Data.SetValue(Valid.ToSingle(value), index); break;
            case TypeCode.String: this.Data.SetValue(Valid.ToString(value), index); break;
            case TypeCode.UInt16: this.Data.SetValue(Valid.ToUInt16(value), index); break;
            case TypeCode.UInt32: this.Data.SetValue(Valid.ToUInt32(value), index); break;
            case TypeCode.UInt64: this.Data.SetValue(Valid.ToUInt64(value), index); break;
        }
    }

}
