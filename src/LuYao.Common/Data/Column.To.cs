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
    public Boolean ToBoolean(int index) => this.Data.ToBoolean(index);

    /// <inheritdoc/>
    public Byte ToByte(int index) => this.Data.ToByte(index);

    /// <inheritdoc/>
    public Char ToChar(int index) => this.Data.ToChar(index);

    /// <inheritdoc/>
    public DateTime ToDateTime(int index) => this.Data.ToDateTime(index);

    /// <inheritdoc/>
    public Decimal ToDecimal(int index) => this.Data.ToDecimal(index);

    /// <inheritdoc/>
    public Double ToDouble(int index) => this.Data.ToDouble(index);

    /// <inheritdoc/>
    public Int16 ToInt16(int index) => this.Data.ToInt16(index);

    /// <inheritdoc/>
    public Int32 ToInt32(int index) => this.Data.ToInt32(index);

    /// <inheritdoc/>
    public Int64 ToInt64(int index) => this.Data.ToInt64(index);

    /// <inheritdoc/>
    public SByte ToSByte(int index) => this.Data.ToSByte(index);

    /// <inheritdoc/>
    public Single ToSingle(int index) => this.Data.ToSingle(index);

    /// <inheritdoc/>
    public String ToString(int index) => this.Data.ToString(index);

    /// <inheritdoc/>
    public UInt16 ToUInt16(int index) => this.Data.ToUInt16(index);

    /// <inheritdoc/>
    public UInt32 ToUInt32(int index) => this.Data.ToUInt32(index);

    /// <inheritdoc/>
    public UInt64 ToUInt64(int index) => this.Data.ToUInt64(index);
}
