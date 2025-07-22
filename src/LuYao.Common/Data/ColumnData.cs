using System;

namespace LuYao.Data;

internal abstract class ColumnData
{
    public abstract void SetValue(object? value, int index);
    public abstract object? GetValue(int index);
    public T GetValue<T>(int index) => (T)this.GetValue(index)!;
    public abstract void Extend(int length);
    public abstract void Clear();
    public virtual Boolean ToBoolean(int index) => throw new NotImplementedException();
    public virtual void Set(Boolean value, int index) => throw new NotImplementedException();

    public virtual Byte ToByte(int index) => throw new NotImplementedException();
    public virtual void Set(Byte value, int index) => throw new NotImplementedException();

    public virtual Char ToChar(int index) => throw new NotImplementedException();
    public virtual void Set(Char value, int index) => throw new NotImplementedException();

    public virtual DateTime ToDateTime(int index) => throw new NotImplementedException();
    public virtual void Set(DateTime value, int index) => throw new NotImplementedException();

    public virtual Decimal ToDecimal(int index) => throw new NotImplementedException();
    public virtual void Set(Decimal value, int index) => throw new NotImplementedException();

    public virtual Double ToDouble(int index) => throw new NotImplementedException();
    public virtual void Set(Double value, int index) => throw new NotImplementedException();

    public virtual Int16 ToInt16(int index) => throw new NotImplementedException();
    public virtual void Set(Int16 value, int index) => throw new NotImplementedException();

    public virtual Int32 ToInt32(int index) => throw new NotImplementedException();
    public virtual void Set(Int32 value, int index) => throw new NotImplementedException();

    public virtual Int64 ToInt64(int index) => throw new NotImplementedException();
    public virtual void Set(Int64 value, int index) => throw new NotImplementedException();

    public virtual SByte ToSByte(int index) => throw new NotImplementedException();
    public virtual void Set(SByte value, int index) => throw new NotImplementedException();

    public virtual Single ToSingle(int index) => throw new NotImplementedException();
    public virtual void Set(Single value, int index) => throw new NotImplementedException();

    public virtual String ToString(int index) => throw new NotImplementedException();
    public virtual void Set(String value, int index) => throw new NotImplementedException();

    public virtual UInt16 ToUInt16(int index) => throw new NotImplementedException();
    public virtual void Set(UInt16 value, int index) => throw new NotImplementedException();

    public virtual UInt32 ToUInt32(int index) => throw new NotImplementedException();
    public virtual void Set(UInt32 value, int index) => throw new NotImplementedException();

    public virtual UInt64 ToUInt64(int index) => throw new NotImplementedException();
    public virtual void Set(UInt64 value, int index) => throw new NotImplementedException();


}

internal class ColumnData<T> : ColumnData
{
    private T[] _data;
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