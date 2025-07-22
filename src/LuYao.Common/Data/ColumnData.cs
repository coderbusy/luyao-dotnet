using System;

namespace LuYao.Data;

internal abstract class ColumnData
{
    public abstract void SetValue(object? value, int index);
    public abstract object? GetValue(int index);
    public T GetValue<T>(int index) => (T)this.GetValue(index)!;
    public abstract void Extend(int length);
    public abstract void Clear();
    public abstract Boolean ToBoolean(int index);
    public abstract void Set(Boolean value, int index);

    public abstract Byte ToByte(int index);
    public abstract void Set(Byte value, int index);

    public abstract Char ToChar(int index);
    public abstract void Set(Char value, int index);

    public abstract DateTime ToDateTime(int index);
    public abstract void Set(DateTime value, int index);

    public abstract Decimal ToDecimal(int index);
    public abstract void Set(Decimal value, int index);

    public abstract Double ToDouble(int index);
    public abstract void Set(Double value, int index);

    public abstract Int16 ToInt16(int index);
    public abstract void Set(Int16 value, int index);

    public abstract Int32 ToInt32(int index);
    public abstract void Set(Int32 value, int index);

    public abstract Int64 ToInt64(int index);
    public abstract void Set(Int64 value, int index);

    public abstract SByte ToSByte(int index);
    public abstract void Set(SByte value, int index);

    public abstract Single ToSingle(int index);
    public abstract void Set(Single value, int index);

    public abstract String ToString(int index);
    public abstract void Set(String value, int index);

    public abstract UInt16 ToUInt16(int index);
    public abstract void Set(UInt16 value, int index);

    public abstract UInt32 ToUInt32(int index);
    public abstract void Set(UInt32 value, int index);

    public abstract UInt64 ToUInt64(int index);
    public abstract void Set(UInt64 value, int index);


}

internal abstract class ColumnData<T> : ColumnData
{
    protected T[] _data;
    public ColumnData(int capacity) => _data = new T[capacity];

    public override void Clear() => Array.Clear(_data, 0, _data.Length);

    public override void Extend(int length)
    {
        if (this._data.Length >= length) return;
        T[] tmp = new T[length];
        this._data.CopyTo(tmp, 0);
        this._data = tmp;
    }

    public override object? GetValue(int index) => this._data[index];

    public override void SetValue(object? value, int index)
    {
        if (value != null)
        {
            this._data[index] = (T)value;
        }
        else
        {
            Array.Clear(this._data, index, 1);
        }
    }
}
