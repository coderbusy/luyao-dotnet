using System;
using System.Collections.Generic;

namespace LuYao.Data;

internal abstract class ColumnData
{
    public abstract void SetValue(object? value, int index);
    public abstract object? GetValue(int index);
    public abstract void Extend(int length);
    public abstract void Clear();

    public virtual object? ToObject(int index) => this.GetValue(index);

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

    public abstract TRet To<TRet>(int index);
}

internal abstract class BaseColumnData<T> : ColumnData
{
    protected T[] _data;
    public BaseColumnData(int capacity) => _data = new T[capacity];

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

    public override TRet To<TRet>(int index)
    {
        object? value = this._data[index];
        if (value is null) return default!;
        if (typeof(TRet) == typeof(T)) return (TRet)value;
        if (value is TRet direct) return direct;
        return (TRet)Valid.To(value, typeof(TRet));
    }
}

sealed class GenericColumnData<T> : BaseColumnData<T>
{
    private static readonly Type ElementType = typeof(T);

    public GenericColumnData(int capacity) : base(capacity)
    {
    }

    public override void Set(bool value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(byte value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(char value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(DateTime value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(decimal value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(double value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(short value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(int value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(long value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(sbyte value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(float value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(string value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(ushort value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(uint value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override void Set(ulong value, int index)
    {
        if (value is T d) this._data[index] = d;
        else this._data[index] = (T)Valid.To(value, ElementType);
    }

    public override bool ToBoolean(int index) => Valid.ToBoolean(this._data[index]);

    public override byte ToByte(int index) => Valid.ToByte(this._data[index]);

    public override char ToChar(int index) => Valid.ToChar(this._data[index]);

    public override DateTime ToDateTime(int index) => Valid.ToDateTime(this._data[index]);

    public override decimal ToDecimal(int index) => Valid.ToDecimal(this._data[index]);

    public override double ToDouble(int index) => Valid.ToDouble(this._data[index]);

    public override short ToInt16(int index) => Valid.ToInt16(this._data[index]);

    public override int ToInt32(int index) => Valid.ToInt32(this._data[index]);

    public override long ToInt64(int index) => Valid.ToInt64(this._data[index]);

    public override sbyte ToSByte(int index) => Valid.ToSByte(this._data[index]);

    public override float ToSingle(int index) => Valid.ToSingle(this._data[index]);

    public override string ToString(int index) => Valid.ToString(this._data[index]);

    public override ushort ToUInt16(int index) => Valid.ToUInt16(this._data[index]);

    public override uint ToUInt32(int index) => Valid.ToUInt32(this._data[index]);

    public override ulong ToUInt64(int index) => Valid.ToUInt64(this._data[index]);
}