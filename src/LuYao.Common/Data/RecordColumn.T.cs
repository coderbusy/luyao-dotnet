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

        // 数组类型返回 JSON 格式
        if (value is Array array)
        {
            return FormatArrayAsJson(array);
        }

        return value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// 将数组格式化为 JSON 字符串。
    /// </summary>
    private static string FormatArrayAsJson(Array array)
    {
        if (array.Length == 0)
        {
            return array.Rank == 1 ? "[]" : FormatMultiDimensionalArray(array);
        }

        if (array.Rank == 1)
        {
            return FormatOneDimensionalArray(array);
        }

        return FormatMultiDimensionalArray(array);
    }

    private static string FormatOneDimensionalArray(Array array)
    {
        var sb = new System.Text.StringBuilder("[");
        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            var element = array.GetValue(i);
            AppendJsonValue(sb, element);
        }
        sb.Append("]");
        return sb.ToString();
    }

    private static string FormatMultiDimensionalArray(Array array)
    {
        var sb = new System.Text.StringBuilder();
        FormatMultiDimensionalArrayRecursive(sb, array, new int[array.Rank], 0);
        return sb.ToString();
    }

    private static void FormatMultiDimensionalArrayRecursive(System.Text.StringBuilder sb, Array array, int[] indices, int dimension)
    {
        sb.Append('[');
        int length = array.GetLength(dimension);
        for (int i = 0; i < length; i++)
        {
            if (i > 0) sb.Append(", ");
            indices[dimension] = i;

            if (dimension == array.Rank - 1)
            {
                var element = array.GetValue(indices);
                AppendJsonValue(sb, element);
            }
            else
            {
                FormatMultiDimensionalArrayRecursive(sb, array, indices, dimension + 1);
            }
        }
        sb.Append(']');
    }

    private static void AppendJsonValue(System.Text.StringBuilder sb, object? value)
    {
        if (value is null)
        {
            sb.Append("null");
        }
        else if (value is string str)
        {
            sb.Append('"');
            // 简单转义（JSON 字符串需要转义特殊字符）
            foreach (char c in str)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default: sb.Append(c); break;
                }
            }
            sb.Append('"');
        }
        else if (value is bool b)
        {
            sb.Append(b ? "true" : "false");
        }
        else if (value is DateTime dt)
        {
            sb.Append('"').Append(dt.ToString("o")).Append('"');
        }
        else if (value is DateTimeOffset dto)
        {
            sb.Append('"').Append(dto.ToString("o")).Append('"');
        }
        else if (value is Guid guid)
        {
            sb.Append('"').Append(guid.ToString()).Append('"');
        }
        else
        {
            sb.Append(value.ToString());
        }
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
