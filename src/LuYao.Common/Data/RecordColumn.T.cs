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
    public RecordColumn(Record record, string name, RecordDataCode code, Type type)
        : base(record, name, code, type)
    {
        var capacity = record.Capacity;
        if (capacity < 0) capacity = 5;
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
        var count = this.Record.Count;
        for (int i = row; i < count - 1; i++)
        {
            //this._data.SetValue(this._data.GetValue(i + 1), i);
            this._data[i] = this._data[i + 1];
        }
    }

    ///<inheritdoc/>
    public override object? GetValue(int row)
    {
        OnGet(row);
        return _data[row];
    }

    ///<inheritdoc/>
    public override void SetValue(object? value, int row)
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

    internal override void Extend(int length)
    {
        if (_data.Length >= length) return;
        T[] tmp = new T[length];
        _data.CopyTo(tmp, 0);
        _data = tmp;
    }

    #region Get / Set

    public T Get(int row)
    {
        OnGet(row);
        return _data[row];
    }

    public T Get() => Get(Record.Cursor);

    public virtual void Set(T value, int row)
    {
        OnSet(row);
        _data[row] = value;
    }

    public void Set(T value) => Set(value, Record.Cursor);

    #endregion
}
