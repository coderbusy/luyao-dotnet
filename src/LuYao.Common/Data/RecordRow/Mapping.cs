using LuYao.Data.Meta;
using System;

namespace LuYao.Data;

partial struct RecordRow
{
    /// <summary>
    /// Maps column values from the current row into the corresponding properties of
    /// <paramref name="data"/>. Uses the runtime type; derived-class properties are handled correctly.
    /// </summary>
    /// <param name="data">The object instance to populate; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public void MapTo(object data)
    {
        XCopy.MapFrom(data, this);
    }

    /// <summary>
    /// Maps column values from the current row into the corresponding properties of
    /// <paramref name="data"/> using the specified options.
    /// </summary>
    /// <param name="data">The object instance to populate; must not be null.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    public void MapTo(object data, RecordMappingOptions options)
    {
        XCopy.MapFrom(data, this, options);
    }

    /// <summary>
    /// Maps readable properties of <paramref name="data"/> into the corresponding columns of
    /// the current row. Uses the runtime type; derived-class properties are handled correctly.
    /// </summary>
    /// <param name="data">The source object; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public void MapFrom(object data) => XCopy.MapTo(data, this);

    /// <summary>
    /// Maps readable properties of <paramref name="data"/> into the corresponding columns of
    /// the current row using the specified options.
    /// </summary>
    /// <param name="data">The source object; must not be null.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    public void MapFrom(object data, RecordMappingOptions options) => XCopy.MapTo(data, this, options);

    /// <summary>
    /// Creates a new <typeparamref name="T"/> instance and populates its properties
    /// from the current row's column values.
    /// </summary>
    /// <typeparam name="T">Target object type; must have a parameterless constructor.</typeparam>
    /// <returns>A new object instance populated with column values.</returns>
    public T To<T>() where T : class, new()
    {
        var ret = new T();
        this.MapTo(ret);
        return ret;
    }

    /// <summary>
    /// Creates a new <typeparamref name="T"/> instance and populates its properties
    /// from the current row's column values using the specified options.
    /// </summary>
    /// <typeparam name="T">Target object type; must have a parameterless constructor.</typeparam>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <returns>A new object instance populated with column values.</returns>
    public T To<T>(RecordMappingOptions options) where T : class, new()
    {
        var ret = new T();
        this.MapTo(ret, options);
        return ret;
    }
}

