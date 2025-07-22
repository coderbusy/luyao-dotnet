using System;

namespace LuYao.Data;

internal abstract class ColumnData
{
    public abstract void SetValue(object? value, int index);
    public abstract object? GetValue(int index);
    public T GetValue<T>(int index) => (T)this.GetValue(index)!;
    public abstract void Extend(int length);
    public abstract void Clear();
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