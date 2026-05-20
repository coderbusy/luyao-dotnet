using LuYao.Data.Mapping;
using LuYao.Data.Meta;
using System;
using System.Collections.Generic;

namespace LuYao.Data;

partial class RecordTable
{
    /// <summary>
    /// Creates a <see cref="RecordTable"/> from a single object, inferring the column structure
    /// automatically and writing one row of data.
    /// </summary>
    /// <typeparam name="T">The source object type.</typeparam>
    /// <param name="data">The object used to infer columns and populate the row.</param>
    /// <returns>A new <see cref="RecordTable"/> containing one row.</returns>
    public static RecordTable From<T>(T data) where T : class
    {
        var re = new RecordTable();
        re.Columns.AddFrom<T>();
        re.AddRowFrom(data);
        return re;
    }

    /// <summary>
    /// Creates a <see cref="RecordTable"/> from a single object using the specified mapping options,
    /// inferring the column structure automatically and writing one row of data.
    /// </summary>
    public static RecordTable From<T>(T data, RecordMappingOptions options) where T : class
    {
        var re = new RecordTable();
        re.Columns.AddFrom<T>(options);
        re.AddRowFrom(data, options);
        return re;
    }

    /// <summary>
    /// Creates a <see cref="RecordTable"/> from a single object, inferring the column structure
    /// automatically, writing one row, and returning a reference to that row.
    /// </summary>
    /// <typeparam name="T">The source object type.</typeparam>
    /// <param name="data">The object used to infer columns and populate the row.</param>
    /// <param name="row">Returns a reference to the newly added row.</param>
    /// <returns>A new <see cref="RecordTable"/> containing one row.</returns>
    public static RecordTable From<T>(T data, out RecordRow row) where T : class
    {
        var re = new RecordTable();
        re.Columns.AddFrom<T>();
        row = re.AddRowFrom(data);
        return re;
    }

    /// <summary>
    /// Creates a <see cref="RecordTable"/> from a single object using the specified mapping options,
    /// inferring the column structure automatically, writing one row, and returning a reference to that row.
    /// </summary>
    /// <typeparam name="T">The source object type.</typeparam>
    /// <param name="data">The object used to infer columns and populate the row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <param name="row">Returns a reference to the newly added row.</param>
    /// <returns>A new <see cref="RecordTable"/> containing one row.</returns>
    public static RecordTable From<T>(T data, RecordMappingOptions options, out RecordRow row) where T : class
    {
        var re = new RecordTable();
        re.Columns.AddFrom<T>(options);
        row = re.AddRowFrom(data, options);
        return re;
    }

    /// <summary>
    /// Converts the first row of the current <see cref="RecordTable"/> to a <typeparamref name="T"/> object.
    /// If the table has no rows, a default instance created by the parameterless constructor is returned.
    /// </summary>
    /// <typeparam name="T">Target object type; must have a parameterless constructor.</typeparam>
    /// <returns>The converted object instance.</returns>
    public T To<T>() where T : class, new()
    {
        var ret = new T();
        if (this.Count > 0)
        {
            XCopy<T>.MapFrom(ret, this[0]);
        }
        return ret;
    }

    /// <summary>
    /// Converts the first row of the current <see cref="RecordTable"/> to a <typeparamref name="T"/> object
    /// using the specified mapping options.
    /// </summary>
    public T To<T>(RecordMappingOptions options) where T : class, new()
    {
        var ret = new T();
        if (this.Count > 0)
        {
            XCopy<T>.MapFrom(ret, this[0], options);
        }
        return ret;
    }

    /// <summary>
    /// Creates a <see cref="RecordTable"/> from a collection of objects, inferring the column
    /// structure automatically and writing each object as one row.
    /// </summary>
    /// <typeparam name="T">The collection element type.</typeparam>
    /// <param name="items">The collection of objects used to populate the rows.</param>
    /// <returns>A new <see cref="RecordTable"/> with the same number of rows as elements in the collection.</returns>
    public static RecordTable FromList<T>(IEnumerable<T> items) where T : class
    {
        var re = new RecordTable();
        re.Columns.AddFrom<T>();
        re.AddRowsFromList(items);
        return re;
    }

    /// <summary>
    /// Creates a <see cref="RecordTable"/> from a collection of objects using the specified mapping options,
    /// inferring the column structure automatically and writing each object as one row.
    /// </summary>
    public static RecordTable FromList<T>(IEnumerable<T> items, RecordMappingOptions options) where T : class
    {
        var re = new RecordTable();
        re.Columns.AddFrom<T>(options);
        re.AddRowsFromList(items, options);
        return re;
    }

    /// <summary>
    /// Appends a new row to the current <see cref="RecordTable"/> and writes the properties of
    /// <paramref name="item"/> into it.
    /// </summary>
    /// <typeparam name="T">The source object type.</typeparam>
    /// <param name="item">The object to append; must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <see langword="null"/>.</exception>
    public RecordRow AddRowFrom<T>(T item) where T : class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var ret = this.AddRow();
        ret.MapFrom(item);
        return ret;
    }

    /// <summary>
    /// Appends a new row to the current <see cref="RecordTable"/> and writes the properties of
    /// <paramref name="item"/> into it using the specified mapping options.
    /// </summary>
    public RecordRow AddRowFrom<T>(T item, RecordMappingOptions options) where T : class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var ret = this.AddRow();
        ret.MapFrom(item, options);
        return ret;
    }

    /// <summary>
    /// Appends one row per element in <paramref name="items"/> to the current <see cref="RecordTable"/>.
    /// </summary>
    /// <typeparam name="T">The collection element type.</typeparam>
    /// <param name="items">The objects to append.</param>
    public void AddRowsFromList<T>(IEnumerable<T> items) where T : class
    {
        foreach (var item in items) this.AddRowFrom(item);
    }

    /// <summary>
    /// Appends one row per element in <paramref name="items"/> to the current <see cref="RecordTable"/>
    /// using the specified mapping options.
    /// </summary>
    public void AddRowsFromList<T>(IEnumerable<T> items, RecordMappingOptions options) where T : class
    {
        foreach (var item in items) this.AddRowFrom(item, options);
    }

    /// <summary>
    /// Converts all rows in the current <see cref="RecordTable"/> to a list of <typeparamref name="T"/> objects.
    /// </summary>
    /// <typeparam name="T">Target object type; must have a parameterless constructor.</typeparam>
    /// <returns>A list with the same number of elements as rows.</returns>
    public List<T> ToList<T>() where T : class, new()
    {
        var list = new List<T>();
        foreach (var row in this)
        {
            var item = row.To<T>();
            list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Converts all rows in the current <see cref="RecordTable"/> to a list of <typeparamref name="T"/> objects
    /// using the specified mapping options.
    /// </summary>
    public List<T> ToList<T>(RecordMappingOptions options) where T : class, new()
    {
        var list = new List<T>();
        foreach (var row in this)
        {
            var item = row.To<T>(options);
            list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Converts the current <see cref="RecordTable"/> to a dictionary keyed by the first column.
    /// Returns an empty dictionary when there are no rows or no columns.
    /// Duplicate keys are overwritten by the later occurrence.
    /// </summary>
    /// <typeparam name="TKey">Key type; corresponds to the value type of the first column.</typeparam>
    /// <typeparam name="T">Value type; must have a parameterless constructor.</typeparam>
    /// <returns>A dictionary keyed by the first column's value with row objects as values.</returns>
    public Dictionary<TKey, T> ToDictionary<TKey, T>()
        where TKey : notnull
        where T : class, new()
    {
        var dict = new Dictionary<TKey, T>();
        if (this.Count == 0 || this.Columns.Count == 0) return dict;
        var keyColumn = this.Columns[0];
        foreach (var row in this)
        {
            var key = keyColumn.To<TKey>(row.Row)!;
            dict[key] = row.To<T>();
        }
        return dict;
    }

    /// <summary>
    /// Converts the current <see cref="RecordTable"/> to a dictionary keyed by the first column,
    /// using the specified mapping options.
    /// </summary>
    public Dictionary<TKey, T> ToDictionary<TKey, T>(RecordMappingOptions options)
        where TKey : notnull
        where T : class, new()
    {
        var dict = new Dictionary<TKey, T>();
        if (this.Count == 0 || this.Columns.Count == 0) return dict;
        var keyColumn = this.Columns[0];
        foreach (var row in this)
        {
            var key = keyColumn.To<TKey>(row.Row)!;
            dict[key] = row.To<T>(options);
        }
        return dict;
    }
}

