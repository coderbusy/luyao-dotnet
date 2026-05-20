using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LuYao.Data.Meta;

/// <summary>
/// Static utility class for bidirectional property mapping between objects and <see cref="RecordRow"/>.
/// Property lists are cached keyed by runtime type, so derived-class properties are handled naturally.
/// </summary>
/// <remarks>
/// All mapping logic is delegated to <see cref="RecordMappingContext"/>.
/// Overloads without options are equivalent to using <see cref="RecordMappingOptions.Default"/>.
/// </remarks>
public static class XCopy
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<XProp>> _readableCache
        = new ConcurrentDictionary<Type, IReadOnlyList<XProp>>();
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<XProp>> _writableCache
        = new ConcurrentDictionary<Type, IReadOnlyList<XProp>>();

    internal static IReadOnlyList<XProp> GetReadableProps(Type runtimeType)
        => _readableCache.GetOrAdd(runtimeType, t =>
        {
            var all = XProp.GetAll(t);
            var list = new List<XProp>(all.Count);
            foreach (var p in all)
            {
                if (Helpers.IsSupportedForReading(p)) list.Add(p);
            }
            return list.AsReadOnly();
        });

    internal static IReadOnlyList<XProp> GetWritableProps(Type runtimeType)
        => _writableCache.GetOrAdd(runtimeType, t =>
        {
            var all = XProp.GetAll(t);
            var list = new List<XProp>(all.Count);
            foreach (var p in all)
            {
                if (Helpers.IsSupportedForWriting(p)) list.Add(p);
            }
            return list.AsReadOnly();
        });

    #region RecordRow

    /// <summary>
    /// Maps readable properties of <paramref name="data"/> into the corresponding columns of
    /// <paramref name="target"/>. Uses the runtime type to scan properties.
    /// Columns that do not exist are silently skipped; no columns are created.
    /// </summary>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static void MapTo(object data, RecordRow target)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        MapTo(data.GetType(), data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// Maps readable properties of the <paramref name="type"/>-declared members on <paramref name="data"/>
    /// into the corresponding columns of <paramref name="target"/>.
    /// Columns that do not exist are silently skipped; no columns are created.
    /// </summary>
    /// <param name="type">Declared type used to scan properties; must not be null.</param>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="data"/> is null.</exception>
    public static void MapTo(Type type, object data, RecordRow target)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        MapTo(type, data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// Maps readable properties of <paramref name="data"/> into the corresponding columns of
    /// <paramref name="target"/> using the specified options.
    /// Columns that do not exist are silently skipped; no columns are created.
    /// </summary>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void MapTo(object data, RecordRow target, RecordMappingOptions options)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        MapTo(data.GetType(), data, target, options);
    }

    /// <summary>
    /// Maps readable properties of the <paramref name="type"/>-declared members on <paramref name="data"/>
    /// into the corresponding columns of <paramref name="target"/> using the specified options.
    /// Columns that do not exist are silently skipped; no columns are created.
    /// </summary>
    /// <param name="type">Declared type used to scan properties; must not be null.</param>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void MapTo(Type type, object data, RecordRow target, RecordMappingOptions options)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).MapDtoToRow(type, data, target);
    }

    /// <summary>
    /// Maps column values from <paramref name="source"/> that correspond to object properties
    /// into <paramref name="data"/>. Uses the runtime type to scan properties.
    /// </summary>
    /// <param name="data">Target object; must not be null.</param>
    /// <param name="source">Source row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static void MapFrom(object data, RecordRow source)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        MapFrom(data.GetType(), data, source, RecordMappingOptions.Default);
    }

    /// <summary>
    /// Maps column values from <paramref name="source"/> that correspond to
    /// <paramref name="type"/>-declared properties into <paramref name="data"/>.
    /// </summary>
    /// <param name="type">Declared type used to scan properties; must not be null.</param>
    /// <param name="data">Target object; must not be null.</param>
    /// <param name="source">Source row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="data"/> is null.</exception>
    public static void MapFrom(Type type, object data, RecordRow source)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        MapFrom(type, data, source, RecordMappingOptions.Default);
    }

    /// <summary>
    /// Maps column values from <paramref name="source"/> into <paramref name="data"/>
    /// using the specified options.
    /// </summary>
    /// <param name="data">Target object; must not be null.</param>
    /// <param name="source">Source row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void MapFrom(object data, RecordRow source, RecordMappingOptions options)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        MapFrom(data.GetType(), data, source, options);
    }

    /// <summary>
    /// Maps column values from <paramref name="source"/> that correspond to
    /// <paramref name="type"/>-declared properties into <paramref name="data"/>
    /// using the specified options (supports custom converters and column-name strategies).
    /// </summary>
    /// <param name="type">Declared type used to scan properties; must not be null.</param>
    /// <param name="data">Target object; must not be null.</param>
    /// <param name="source">Source row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void MapFrom(Type type, object data, RecordRow source, RecordMappingOptions options)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).MapRowToDto(type, data, source);
    }

    /// <summary>
    /// Writes readable properties of <paramref name="data"/> into the corresponding columns of
    /// <paramref name="target"/>, creating missing columns automatically.
    /// Uses the runtime type to scan properties.
    /// </summary>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static void WriteTo(object data, RecordRow target)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteTo(data.GetType(), data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// Writes readable properties of the <paramref name="type"/>-declared members on <paramref name="data"/>
    /// into the corresponding columns of <paramref name="target"/>, creating missing columns automatically.
    /// </summary>
    /// <param name="type">Declared type used to scan properties; must not be null.</param>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="data"/> is null.</exception>
    public static void WriteTo(Type type, object data, RecordRow target)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteTo(type, data, target, RecordMappingOptions.Default);
    }

    /// <summary>
    /// Writes readable properties of <paramref name="data"/> into the corresponding columns of
    /// <paramref name="target"/> (auto-creating columns), using the specified options.
    /// </summary>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void WriteTo(object data, RecordRow target, RecordMappingOptions options)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteTo(data.GetType(), data, target, options);
    }

    /// <summary>
    /// Writes readable properties of the <paramref name="type"/>-declared members on <paramref name="data"/>
    /// into the corresponding columns of <paramref name="target"/> (auto-creating columns),
    /// using the specified options.
    /// </summary>
    /// <param name="type">Declared type used to scan properties; must not be null.</param>
    /// <param name="data">Source object; must not be null.</param>
    /// <param name="target">Target row.</param>
    /// <param name="options">Mapping options; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void WriteTo(Type type, object data, RecordRow target, RecordMappingOptions options)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).WriteDtoToRow(type, data, target);
    }

    #endregion
}

/// <summary>
/// Generic thin wrapper over <see cref="XCopy"/> that fixes the scan type to the
/// compile-time type <typeparamref name="T"/>. All logic is delegated to the explicit
/// <see cref="Type"/> overloads of <see cref="XCopy"/>.
/// </summary>
/// <typeparam name="T">The object type to map; must be a reference type.</typeparam>
public static class XCopy<T> where T : class
{
    #region RecordRow
    /// <summary>
    /// Maps readable properties of <paramref name="data"/> into the corresponding columns of
    /// <paramref name="target"/>. Always scans using the compile-time type <typeparamref name="T"/>;
    /// derived-class properties are not processed even if the runtime type is a subclass.
    /// </summary>
    /// <param name="data">Source object.</param>
    /// <param name="target">Target row; columns that do not exist are silently skipped, <b>no columns are created</b>.</param>
    public static void MapTo(T data, RecordRow target) => XCopy.MapTo(typeof(T), data, target);

    /// <summary>Maps readable properties of <paramref name="data"/> into the corresponding columns of <paramref name="target"/> using the specified options.</summary>
    public static void MapTo(T data, RecordRow target, RecordMappingOptions options) => XCopy.MapTo(typeof(T), data, target, options);

    /// <summary>
    /// Maps column values from <paramref name="source"/> that match object properties
    /// into <paramref name="data"/>. Always scans using the compile-time type <typeparamref name="T"/>;
    /// derived-class properties are not processed even if the runtime type is a subclass.
    /// </summary>
    /// <param name="data">Target object.</param>
    /// <param name="source">Source row.</param>
    public static void MapFrom(T data, RecordRow source) => XCopy.MapFrom(typeof(T), data, source);

    /// <summary>Maps column values from <paramref name="source"/> into <paramref name="data"/> using the specified options.</summary>
    public static void MapFrom(T data, RecordRow source, RecordMappingOptions options) => XCopy.MapFrom(typeof(T), data, source, options);

    /// <summary>
    /// Writes readable properties of <paramref name="data"/> into the corresponding columns of
    /// <paramref name="target"/>, creating missing columns automatically.
    /// Always scans using the compile-time type <typeparamref name="T"/>;
    /// derived-class properties are not processed even if the runtime type is a subclass.
    /// </summary>
    /// <param name="data">Source object.</param>
    /// <param name="target">Target row.</param>
    public static void WriteTo(T data, RecordRow target) => XCopy.WriteTo(typeof(T), data, target);

    /// <summary>Writes readable properties of <paramref name="data"/> into the corresponding columns of <paramref name="target"/> (auto-creating columns) using the specified options.</summary>
    public static void WriteTo(T data, RecordRow target, RecordMappingOptions options) => XCopy.WriteTo(typeof(T), data, target, options);

    #endregion
}

/// <summary>
/// Static utility class for shallow-copying between two strongly-typed objects by property name.
/// Only properties that exist in both types with the same name, the same type, and are supported
/// for reading/writing are copied.
/// </summary>
/// <typeparam name="TSource">The source object type.</typeparam>
/// <typeparam name="TTarget">The target object type; must have a parameterless constructor.</typeparam>
public static class XCopy<TSource, TTarget> where TSource : class where TTarget : class, new()
{
    // Pre-build the source→target property-pair map to avoid repeated lookups on every call.
    private static readonly IReadOnlyList<PropPair> _map = BuildMap();

    private readonly struct PropPair
    {
        internal readonly IXProp Source;
        internal readonly IXProp Target;

        internal PropPair(IXProp source, IXProp target)
        {
            Source = source;
            Target = target;
        }

        public void Copy(TSource sourceInstance, TTarget targetInstance)
        {
            Target.SetValue(targetInstance, Source.GetValue(sourceInstance));
        }
    }

    private static IReadOnlyList<PropPair> BuildMap()
    {
        var sourceProps = XProp.GetAll(typeof(TSource));
        var targetProps = XProp.GetAll(typeof(TTarget));

        // Build a fast-lookup index keyed by target property name.
        var targetIndex = new Dictionary<string, IXProp>(StringComparer.Ordinal);
        foreach (var tp in targetProps)
        {
            if (tp.CanWrite) targetIndex[tp.Name] = tp;
        }

        var map = new List<PropPair>();
        foreach (var sp in sourceProps)
        {
            if (!sp.CanRead) continue;
            if (!targetIndex.TryGetValue(sp.Name, out var tp)) continue;
            // Only map properties whose types match exactly.
            if (sp.Type != tp.Type) continue;
            map.Add(new PropPair(sp, tp));
        }

        return map.AsReadOnly();
    }

    /// <summary>
    /// Creates a new <typeparamref name="TTarget"/> instance and copies the properties from
    /// <paramref name="source"/> that share the same name, same type, and are supported.
    /// </summary>
    /// <param name="source">Source object; must not be null.</param>
    /// <returns>A new target object populated with values from the source.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static TTarget Copy(TSource source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var target = new TTarget();
        CopyTo(source, target);
        return target;
    }

    /// <summary>
    /// Copies supported property values from <paramref name="source"/> into an existing
    /// <paramref name="target"/> instance.
    /// </summary>
    /// <param name="source">Source object; must not be null.</param>
    /// <param name="target">Target object; must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void CopyTo(TSource source, TTarget target)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (target == null) throw new ArgumentNullException(nameof(target));

        foreach (var pair in _map)
        {
            pair.Copy(source, target);
        }
    }
}
