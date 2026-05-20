using System;

namespace LuYao.Data;

/// <summary>
/// 泛型列
/// </summary>
/// <typeparam name="T"></typeparam>
public class RecordColumn<T> : RecordColumn
{
    /// <summary>
    /// 数据存储数组
    /// </summary>
    protected internal T[] _data;

    /// <summary>
    /// 创建一个泛型列
    /// </summary>
    public RecordColumn(RecordTable table, string name, Type type)
        : base(table, name, type)
    {
        var capacity = table.Capacity;
        if (capacity <= 0) capacity = 5;
        _data = new T[capacity];
    }

    ///<inheritdoc/>
    public override int Capacity => _data.Length;

    ///<inheritdoc/>
    public override void Clear()
    {
        Array.Clear(_data, 0, _data.Length);
    }

    ///<inheritdoc/>
    public override void Delete(int row)
    {
        OnGet(row);
        var count = this.Table.Count;
        for (int i = row; i < count - 1; i++)
        {
            this._data[i] = this._data[i + 1];
        }
    }

    ///<inheritdoc/>
    public override object? Get(int row)
    {
        OnGet(row);
        return _data[row];
    }

    ///<inheritdoc/>
    public override void Set(int row, object? value)
    {
        OnSet(row);
        if (value is null)
        {
            Array.Clear(_data, row, 1);
        }
        else if (value is T direct)
        {
            _data[row] = direct;
        }
        else
        {
            _data[row] = (T)Valid.To(value, this.Type);
        }
    }

    ///<inheritdoc/>
    public override string ToString(int row)
    {
        OnGet(row);
        T value = _data[row];
        if (value is null) return string.Empty;

        // byte[] columns return Base64 string
        if (value is byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        return value.ToString() ?? string.Empty;
    }


    internal override void Extend(int length)
    {
        if (_data.Length >= length) return;
        int newLen = Math.Max(length, _data.Length * 2);
        T[] tmp = new T[newLen];
        _data.CopyTo(tmp, 0);
        _data = tmp;
    }

    internal override Array GetDataArray(int count)
    {
        if (count == _data.Length) return _data;
        var arr = new T[count];
        Array.Copy(_data, arr, count);
        return arr;
    }

    internal override void SetDataArray(Array data, int count)
    {
        var src = (T[])data;
        var len = Math.Min(src.Length, count);
        if (_data.Length < count) _data = new T[count];
        Array.Copy(src, _data, len);
    }

    internal override void Reorder(int[] indices)
    {
        int count = indices.Length;
        T[] tmp = new T[_data.Length];
        for (int i = 0; i < count; i++)
        {
            tmp[i] = _data[indices[i]];
        }
        _data = tmp;
    }

    #region Accessor

    ///<inheritdoc/>
    public T GetValue(int row)
    {
        OnGet(row);
        return _data[row];
    }

    ///<inheritdoc/>
    public virtual void SetValue(int row, T value)
    {
        OnSet(row);
        _data[row] = value;
    }

    #endregion
}
