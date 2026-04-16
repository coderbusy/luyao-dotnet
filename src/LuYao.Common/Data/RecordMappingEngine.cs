using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LuYao.Data;

/// <summary>
/// 对象映射核心引擎，基于表达式树编译生成强类型委托。
/// </summary>
internal static class RecordMappingEngine
{
    #region Binding types (net45 compatible, no ValueTuple)

    private sealed class ParamBinding
    {
        public ParameterInfo Param { get; }
        public string ColumnName { get; }
        public ParamBinding(ParameterInfo param, string columnName) { Param = param; ColumnName = columnName; }
    }

    private sealed class PropBinding
    {
        public PropertyInfo Prop { get; }
        public string ColumnName { get; }
        public PropBinding(PropertyInfo prop, string columnName) { Prop = prop; ColumnName = columnName; }
    }

    private sealed class WriteBinding
    {
        public PropertyInfo Property { get; }
        public string ColumnName { get; }
        public WriteBinding(PropertyInfo property, string columnName) { Property = property; ColumnName = columnName; }
    }

    #endregion

    #region Cache

    private readonly struct CacheKey : IEquatable<CacheKey>
    {
        public readonly Type Type;
        public readonly string SchemaSignature;
        public readonly StringComparison NameComparison;
        public readonly bool AutoAddColumns;
        public readonly bool RequireAllProperties;

        public CacheKey(Type type, string schemaSignature, RecordMappingOptions options)
        {
            Type = type;
            SchemaSignature = schemaSignature;
            NameComparison = options != null ? options.NameComparison : StringComparison.OrdinalIgnoreCase;
            AutoAddColumns = options != null && options.AutoAddColumns;
            RequireAllProperties = options != null && options.RequireAllProperties;
        }

        public bool Equals(CacheKey other)
        {
            return Type == other.Type
                && SchemaSignature == other.SchemaSignature
                && NameComparison == other.NameComparison
                && AutoAddColumns == other.AutoAddColumns
                && RequireAllProperties == other.RequireAllProperties;
        }

        public override bool Equals(object obj) => obj is CacheKey other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Type.GetHashCode();
                hash = hash * 397 ^ SchemaSignature.GetHashCode();
                hash = hash * 397 ^ (int)NameComparison;
                hash = hash * 397 ^ AutoAddColumns.GetHashCode();
                hash = hash * 397 ^ RequireAllProperties.GetHashCode();
                return hash;
            }
        }
    }

    private static readonly ConcurrentDictionary<CacheKey, object> _writePlanCache = new ConcurrentDictionary<CacheKey, object>();
    private static readonly ConcurrentDictionary<CacheKey, object> _readPlanCache = new ConcurrentDictionary<CacheKey, object>();

    private static string GetSchemaSignature(Record record)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < record.Columns.Count; i++)
        {
            if (i > 0) sb.Append('|');
            var col = record.Columns[i];
            sb.Append(col.Name).Append(':').Append(col.Type.FullName);
        }
        return sb.ToString();
    }

    #endregion

    #region Write (AddRow)

    internal static void WriteRow<T>(T item, Record record, int row, RecordMappingOptions options)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        if (options != null && options.Mapper != null)
        {
            var typedMapper = options.Mapper as IRecordMapper<T>;
            if (typedMapper != null)
                typedMapper.Write(item, record, row);
            else
                options.Mapper.Write(item, record, row);
            return;
        }

        var plan = GetOrCreateWritePlan<T>(record, options);
        plan.Execute(item, record, row, options);
    }

    private static WritePlan<T> GetOrCreateWritePlan<T>(Record record, RecordMappingOptions options)
    {
        var key = new CacheKey(typeof(T), GetSchemaSignature(record), options);
        return (WritePlan<T>)_writePlanCache.GetOrAdd(key, _ => BuildWritePlan<T>(record, options));
    }

    private static WritePlan<T> BuildWritePlan<T>(Record record, RecordMappingOptions options)
    {
        var comparison = options != null ? options.NameComparison : StringComparison.OrdinalIgnoreCase;
        var nameTransform = options != null ? options.NameTransform : null;
        var autoAddColumns = options != null && options.AutoAddColumns;

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanRead);

        var bindings = new List<WriteBinding>();

        foreach (var prop in props)
        {
            if (!IsSimpleMappableType(prop.PropertyType))
                continue;

            var columnName = nameTransform != null ? nameTransform(prop.Name) : prop.Name;
            bindings.Add(new WriteBinding(prop, columnName));
        }

        return new WritePlan<T>(bindings, comparison, autoAddColumns);
    }

    private sealed class WritePlan<T>
    {
        private readonly List<WriteBinding> _bindings;
        private readonly StringComparison _comparison;
        private readonly bool _autoAddColumns;

        private volatile Action<T, RecordColumn[], int> _compiledWriter;
        private volatile int[] _columnIndices;
        private volatile string _lastSchemaSignature;

        public WritePlan(List<WriteBinding> bindings, StringComparison comparison, bool autoAddColumns)
        {
            _bindings = bindings;
            _comparison = comparison;
            _autoAddColumns = autoAddColumns;
        }

        public void Execute(T item, Record record, int row, RecordMappingOptions options)
        {
            var serializeValue = options != null ? options.SerializeValue : null;

            if (serializeValue == null && !_autoAddColumns)
            {
                EnsureCompiled(record);
                if (_compiledWriter != null && _columnIndices != null)
                {
                    var cols = new RecordColumn[_columnIndices.Length];
                    for (int i = 0; i < _columnIndices.Length; i++)
                    {
                        var idx = _columnIndices[i];
                        cols[i] = idx >= 0 ? record.Columns[idx] : null;
                    }
                    _compiledWriter(item, cols, row);
                    return;
                }
            }

            // Slow path
            foreach (var binding in _bindings)
            {
                var col = FindColumnByComparison(record, binding.ColumnName, _comparison);
                if (col == null)
                {
                    if (_autoAddColumns && IsSimpleMappableType(binding.Property.PropertyType))
                    {
                        var colType = GetColumnTypeForProperty(binding.Property.PropertyType);
                        col = record.Columns.Add(binding.ColumnName, colType);
                    }
                    else continue;
                }

                var value = binding.Property.GetValue(item, null);
                if (serializeValue != null)
                    value = serializeValue(binding.ColumnName, col.Type, value);
                else
                    value = ConvertForWrite(value, col.Type, binding.Property.PropertyType);

                col.SetValue(value, row);
            }
        }

        private void EnsureCompiled(Record record)
        {
            var sig = GetSchemaSignature(record);
            if (sig == _lastSchemaSignature && _compiledWriter != null) return;

            var matchedBindings = new List<int>(); // indices into _bindings
            var matchedColIndices = new List<int>();
            for (int i = 0; i < _bindings.Count; i++)
            {
                int colIdx = FindColumnIndex(record, _bindings[i].ColumnName, _comparison);
                if (colIdx >= 0)
                {
                    matchedBindings.Add(i);
                    matchedColIndices.Add(colIdx);
                }
            }

            if (matchedBindings.Count == 0)
            {
                _compiledWriter = null;
                _columnIndices = null;
                _lastSchemaSignature = sig;
                return;
            }

            var itemParam = Expression.Parameter(typeof(T), "item");
            var colsParam = Expression.Parameter(typeof(RecordColumn[]), "cols");
            var rowParam = Expression.Parameter(typeof(int), "row");

            var body = new List<Expression>();
            var indices = new int[matchedBindings.Count];

            for (int i = 0; i < matchedBindings.Count; i++)
            {
                var binding = _bindings[matchedBindings[i]];
                indices[i] = matchedColIndices[i];

                var col = record.Columns[matchedColIndices[i]];
                var propExpr = Expression.Property(itemParam, binding.Property);
                var colExpr = Expression.ArrayIndex(colsParam, Expression.Constant(i));

                var propType = binding.Property.PropertyType;
                var colType = col.Type;

                if (propType == colType)
                {
                    var typedCol = Expression.Convert(colExpr, typeof(RecordColumn<>).MakeGenericType(colType));
                    var setMethod = typeof(RecordColumn<>).MakeGenericType(colType)
                        .GetMethod("Set", new[] { colType, typeof(int) });
                    body.Add(Expression.Call(typedCol, setMethod, propExpr, rowParam));
                    continue;
                }

                Expression valueExpr;
                var underlyingProp = Nullable.GetUnderlyingType(propType);
                if (underlyingProp != null)
                {
                    var hasValue = Expression.Property(propExpr, "HasValue");
                    var rawValue = Expression.Property(propExpr, "Value");
                    valueExpr = Expression.Condition(
                        hasValue,
                        Expression.Convert(rawValue, typeof(object)),
                        Expression.Constant(null, typeof(object)));
                }
                else
                {
                    valueExpr = Expression.Convert(propExpr, typeof(object));
                }

                var setValueMethod = typeof(RecordColumn).GetMethod("SetValue", new[] { typeof(object), typeof(int) });
                body.Add(Expression.Call(colExpr, setValueMethod, valueExpr, rowParam));
            }

            if (body.Count > 0)
            {
                var block = Expression.Block(body);
                _compiledWriter = Expression.Lambda<Action<T, RecordColumn[], int>>(block, itemParam, colsParam, rowParam).Compile();
            }
            _columnIndices = indices;
            _lastSchemaSignature = sig;
        }
    }

    #endregion

    #region Read (ToList / To)

    internal static T ReadRow<T>(Record record, int row, RecordMappingOptions options)
    {
        if (options != null && options.Mapper != null)
        {
            var typedMapper = options.Mapper as IRecordMapper<T>;
            if (typedMapper != null)
                return typedMapper.Read(record, row);
            return (T)options.Mapper.Read(record, row);
        }

        var plan = GetOrCreateReadPlan<T>(record, options);
        return plan.Execute(record, row, options);
    }

    internal static List<T> ReadAll<T>(Record record, RecordMappingOptions options)
    {
        if (options != null && options.Mapper != null)
        {
            var list = new List<T>(record.Count);
            var typedMapper = options.Mapper as IRecordMapper<T>;
            if (typedMapper != null)
            {
                for (int i = 0; i < record.Count; i++)
                    list.Add(typedMapper.Read(record, i));
            }
            else
            {
                for (int i = 0; i < record.Count; i++)
                    list.Add((T)options.Mapper.Read(record, i));
            }
            return list;
        }

        var plan = GetOrCreateReadPlan<T>(record, options);
        var result = new List<T>(record.Count);
        for (int i = 0; i < record.Count; i++)
            result.Add(plan.Execute(record, i, options));
        return result;
    }

    private static ReadPlan<T> GetOrCreateReadPlan<T>(Record record, RecordMappingOptions options)
    {
        var key = new CacheKey(typeof(T), GetSchemaSignature(record), options);
        return (ReadPlan<T>)_readPlanCache.GetOrAdd(key, _ => BuildReadPlan<T>(record, options));
    }

    private static ReadPlan<T> BuildReadPlan<T>(Record record, RecordMappingOptions options)
    {
        var comparison = options != null ? options.NameComparison : StringComparison.OrdinalIgnoreCase;
        var nameTransform = options != null ? options.NameTransform : null;
        var requireAll = options != null && options.RequireAllProperties;

        var type = typeof(T);
        var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        // 1. [RecordConstructor]
        var markedCtors = ctors.Where(c => c.GetCustomAttributes(typeof(RecordConstructorAttribute), false).Length > 0).ToArray();
        if (markedCtors.Length > 1)
            throw new AmbiguousMatchException(string.Format("类型 '{0}' 标记了多个 [RecordConstructor]，最多只能标记一个。", type.FullName));

        if (markedCtors.Length == 1)
            return BuildCtorPlan<T>(record, markedCtors[0], comparison, nameTransform, requireAll);

        // 2. Parameterless ctor
        var parameterlessCtor = ctors.FirstOrDefault(c => c.GetParameters().Length == 0);
        if (parameterlessCtor != null)
            return BuildCtorPlan<T>(record, parameterlessCtor, comparison, nameTransform, requireAll);

        // 3. Single RecordRow ctor
        var recordRowCtor = ctors.FirstOrDefault(c =>
        {
            var ps = c.GetParameters();
            return ps.Length == 1 && ps[0].ParameterType == typeof(RecordRow);
        });
        if (recordRowCtor != null)
            return new RecordRowCtorPlan<T>(recordRowCtor);

        throw new MissingMethodException(string.Format("类型 '{0}' 没有可用的构造函数。需要：[RecordConstructor] 标记的构造函数、无参构造函数、或接受单个 RecordRow 参数的构造函数。", type.FullName));
    }

    private static ReadPlan<T> BuildCtorPlan<T>(
        Record record, ConstructorInfo ctor,
        StringComparison comparison, Func<string, string> nameTransform,
        bool requireAll)
    {
        var type = typeof(T);
        var ctorParams = ctor.GetParameters();

        var paramBindings = new List<ParamBinding>();
        foreach (var p in ctorParams)
            paramBindings.Add(new ParamBinding(p, nameTransform != null ? nameTransform(p.Name) : p.Name));

        var ctorParamNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in ctorParams)
            ctorParamNames.Add(p.Name);

        var propBindings = new List<PropBinding>();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanWrite && !ctorParamNames.Contains(p.Name));

        foreach (var prop in props)
        {
            if (!IsSimpleMappableType(prop.PropertyType)) continue;
            var colName = nameTransform != null ? nameTransform(prop.Name) : prop.Name;
            propBindings.Add(new PropBinding(prop, colName));
        }

        if (requireAll)
        {
            var allProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => p.CanWrite && IsSimpleMappableType(p.PropertyType));
            foreach (var prop in allProps)
            {
                var colName = nameTransform != null ? nameTransform(prop.Name) : prop.Name;
                if (FindColumnByComparison(record, colName, comparison) == null)
                {
                    bool inCtorParam = paramBindings.Any(pb => string.Equals(pb.ColumnName, colName, comparison));
                    if (!inCtorParam)
                        throw new InvalidOperationException(string.Format("属性 '{0}' 没有匹配的列 '{1}'。", prop.Name, colName));
                }
            }
        }

        return new CompiledReadPlan<T>(ctor, paramBindings, propBindings, comparison);
    }

    private abstract class ReadPlan<T>
    {
        public abstract T Execute(Record record, int row, RecordMappingOptions options);
    }

    private sealed class RecordRowCtorPlan<T> : ReadPlan<T>
    {
        private readonly Func<RecordRow, T> _factory;

        public RecordRowCtorPlan(ConstructorInfo ctor)
        {
            var rowParam = Expression.Parameter(typeof(RecordRow), "row");
            var newExpr = Expression.New(ctor, rowParam);
            _factory = Expression.Lambda<Func<RecordRow, T>>(newExpr, rowParam).Compile();
        }

        public override T Execute(Record record, int row, RecordMappingOptions options)
        {
            return _factory(new RecordRow(record, row));
        }
    }

    private sealed class CompiledReadPlan<T> : ReadPlan<T>
    {
        private readonly ConstructorInfo _ctor;
        private readonly List<ParamBinding> _paramBindings;
        private readonly List<PropBinding> _propBindings;
        private readonly StringComparison _comparison;

        private volatile Func<RecordColumn[], int, T> _compiledReader;
        private volatile int[] _paramColumnIndices;
        private volatile int[] _propColumnIndices;
        private volatile string _lastSchemaSignature;

        public CompiledReadPlan(
            ConstructorInfo ctor,
            List<ParamBinding> paramBindings,
            List<PropBinding> propBindings,
            StringComparison comparison)
        {
            _ctor = ctor;
            _paramBindings = paramBindings;
            _propBindings = propBindings;
            _comparison = comparison;
        }

        public override T Execute(Record record, int row, RecordMappingOptions options)
        {
            var deserializeValue = options != null ? options.DeserializeValue : null;

            if (deserializeValue == null)
            {
                EnsureCompiled(record);
                if (_compiledReader != null)
                {
                    int totalCols = _paramColumnIndices.Length + _propColumnIndices.Length;
                    var cols = new RecordColumn[totalCols];
                    for (int i = 0; i < _paramColumnIndices.Length; i++)
                    {
                        var idx = _paramColumnIndices[i];
                        cols[i] = idx >= 0 ? record.Columns[idx] : null;
                    }
                    for (int i = 0; i < _propColumnIndices.Length; i++)
                    {
                        var idx = _propColumnIndices[i];
                        cols[_paramColumnIndices.Length + i] = idx >= 0 ? record.Columns[idx] : null;
                    }
                    return _compiledReader(cols, row);
                }
            }

            return ExecuteSlow(record, row, deserializeValue);
        }

        private T ExecuteSlow(Record record, int row, Func<string, Type, object, object> deserializeValue)
        {
            var args = new object[_paramBindings.Count];
            for (int i = 0; i < _paramBindings.Count; i++)
            {
                var pb = _paramBindings[i];
                var col = FindColumnByComparison(record, pb.ColumnName, _comparison);
                if (col != null)
                {
                    var value = col.GetValue(row);
                    if (deserializeValue != null)
                        value = deserializeValue(pb.ColumnName, pb.Param.ParameterType, value);
                    args[i] = ConvertForRead(value, pb.Param.ParameterType, pb.ColumnName, row);
                }
                else
                {
                    args[i] = GetDefault(pb.Param.ParameterType);
                }
            }

            var obj = (T)_ctor.Invoke(args);

            foreach (var pb in _propBindings)
            {
                var col = FindColumnByComparison(record, pb.ColumnName, _comparison);
                if (col == null) continue;

                var value = col.GetValue(row);
                if (deserializeValue != null)
                    value = deserializeValue(pb.ColumnName, pb.Prop.PropertyType, value);
                var converted = ConvertForRead(value, pb.Prop.PropertyType, pb.ColumnName, row);
                pb.Prop.SetValue(obj, converted, null);
            }

            return obj;
        }

        private void EnsureCompiled(Record record)
        {
            var sig = GetSchemaSignature(record);
            if (sig == _lastSchemaSignature && _compiledReader != null) return;

            var paramIndices = new int[_paramBindings.Count];
            for (int i = 0; i < _paramBindings.Count; i++)
                paramIndices[i] = FindColumnIndex(record, _paramBindings[i].ColumnName, _comparison);

            var propIndices = new int[_propBindings.Count];
            for (int i = 0; i < _propBindings.Count; i++)
                propIndices[i] = FindColumnIndex(record, _propBindings[i].ColumnName, _comparison);

            var colsParam = Expression.Parameter(typeof(RecordColumn[]), "cols");
            var rowParam = Expression.Parameter(typeof(int), "row");

            int colArrayIndex = 0;

            // Constructor arguments
            var ctorArgs = new Expression[_paramBindings.Count];
            for (int i = 0; i < _paramBindings.Count; i++)
            {
                var param = _paramBindings[i].Param;
                if (paramIndices[i] >= 0)
                {
                    ctorArgs[i] = BuildReadExpression(colsParam, colArrayIndex, rowParam,
                        record.Columns[paramIndices[i]].Type, param.ParameterType);
                }
                else
                {
                    ctorArgs[i] = Expression.Default(param.ParameterType);
                }
                colArrayIndex++;
            }

            var newExpr = Expression.New(_ctor, ctorArgs);

            if (_propBindings.Count == 0)
            {
                _compiledReader = Expression.Lambda<Func<RecordColumn[], int, T>>(newExpr, colsParam, rowParam).Compile();
            }
            else
            {
                var memberBindings = new List<MemberBinding>();
                for (int i = 0; i < _propBindings.Count; i++)
                {
                    var prop = _propBindings[i].Prop;
                    if (propIndices[i] >= 0)
                    {
                        var valueExpr = BuildReadExpression(colsParam, colArrayIndex, rowParam,
                            record.Columns[propIndices[i]].Type, prop.PropertyType);
                        memberBindings.Add(Expression.Bind(prop, valueExpr));
                    }
                    colArrayIndex++;
                }

                if (memberBindings.Count > 0)
                {
                    var initExpr = Expression.MemberInit(newExpr, memberBindings);
                    _compiledReader = Expression.Lambda<Func<RecordColumn[], int, T>>(initExpr, colsParam, rowParam).Compile();
                }
                else
                {
                    _compiledReader = Expression.Lambda<Func<RecordColumn[], int, T>>(newExpr, colsParam, rowParam).Compile();
                }
            }

            _paramColumnIndices = paramIndices;
            _propColumnIndices = propIndices;
            _lastSchemaSignature = sig;
        }
    }

    #endregion

    #region Utilities

    private static Expression BuildReadExpression(
        ParameterExpression colsParam, int arrayIndex,
        ParameterExpression rowParam, Type columnType, Type targetType)
    {
        var colExpr = Expression.ArrayIndex(colsParam, Expression.Constant(arrayIndex));

        if (columnType == targetType)
        {
            var typedCol = Expression.Convert(colExpr, typeof(RecordColumn<>).MakeGenericType(columnType));
            var getMethod = typeof(RecordColumn<>).MakeGenericType(columnType)
                .GetMethod("Get", new[] { typeof(int) });
            return Expression.Call(typedCol, getMethod, rowParam);
        }

        var getValueMethod = typeof(RecordColumn).GetMethod("GetValue", new[] { typeof(int) });
        var rawValue = Expression.Call(colExpr, getValueMethod, rowParam);
        return BuildConvertExpression(rawValue, targetType);
    }

    private static Expression BuildConvertExpression(Expression valueExpr, Type targetType)
    {
        var underlyingTarget = Nullable.GetUnderlyingType(targetType);
        var isNullable = underlyingTarget != null;
        var effectiveTarget = underlyingTarget ?? targetType;

        if (effectiveTarget.IsEnum)
        {
            var convertMethod = typeof(RecordMappingEngine).GetMethod(nameof(ConvertEnum), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(effectiveTarget);
            Expression result = Expression.Call(convertMethod, valueExpr);
            if (isNullable)
                result = Expression.Convert(result, targetType);
            return result;
        }

        var convertForRead = typeof(RecordMappingEngine).GetMethod(nameof(ConvertForReadStatic), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(targetType);
        return Expression.Call(convertForRead, valueExpr);
    }

    private static TEnum ConvertEnum<TEnum>(object value) where TEnum : struct
    {
        if (value == null) return default(TEnum);
        if (value is string s) return (TEnum)Enum.Parse(typeof(TEnum), s, ignoreCase: true);
        return (TEnum)Enum.ToObject(typeof(TEnum), value);
    }

    private static T ConvertForReadStatic<T>(object value)
    {
        if (value == null) return default(T);
        if (value is T direct) return direct;

        var targetType = typeof(T);
        var underlying = Nullable.GetUnderlyingType(targetType);
        var effectiveType = underlying ?? targetType;

        if (effectiveType.IsEnum)
        {
            if (value is string s) return (T)Enum.Parse(effectiveType, s, ignoreCase: true);
            return (T)Enum.ToObject(effectiveType, value);
        }

        return (T)Valid.To(value, effectiveType);
    }

    internal static RecordColumn FindColumnByComparison(Record record, string columnName, StringComparison comparison)
    {
        for (int i = 0; i < record.Columns.Count; i++)
        {
            if (string.Equals(record.Columns[i].Name, columnName, comparison))
                return record.Columns[i];
        }
        return null;
    }

    private static int FindColumnIndex(Record record, string columnName, StringComparison comparison)
    {
        for (int i = 0; i < record.Columns.Count; i++)
        {
            if (string.Equals(record.Columns[i].Name, columnName, comparison))
                return i;
        }
        return -1;
    }

    private static object ConvertForWrite(object value, Type columnType, Type propertyType)
    {
        if (value == null) return null;
        if (value.GetType() == columnType) return value;

        if (propertyType.IsEnum)
        {
            if (columnType == typeof(string))
                return value.ToString();
            return Convert.ChangeType(value, Enum.GetUnderlyingType(propertyType));
        }

        return Valid.To(value, columnType);
    }

    private static object ConvertForRead(object value, Type targetType, string columnName, int row)
    {
        if (value == null) return GetDefault(targetType);

        var underlying = Nullable.GetUnderlyingType(targetType);
        var effectiveType = underlying ?? targetType;

        try
        {
            if (value.GetType() == effectiveType) return value;

            if (effectiveType.IsEnum)
            {
                if (value is string s) return Enum.Parse(effectiveType, s, ignoreCase: true);
                return Enum.ToObject(effectiveType, value);
            }

            return Valid.To(value, effectiveType);
        }
        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
        {
            throw new InvalidCastException(
                string.Format("列 '{0}' 的值无法转换为 '{1}'（源类型: {2}，行: {3}）。", columnName, targetType.Name, value.GetType().Name, row), ex);
        }
    }

    private static object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static bool IsSimpleMappableType(Type type)
    {
        if (Helpers.IsSupportedColumnType(type)) return true;
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null && Helpers.IsSupportedColumnType(underlying)) return true;
        if (type.IsEnum) return true;
        if (underlying != null && underlying.IsEnum) return true;
        return false;
    }

    private static Type GetColumnTypeForProperty(Type propertyType)
    {
        if (propertyType.IsEnum) return typeof(int);
        var underlying = Nullable.GetUnderlyingType(propertyType);
        if (underlying != null && underlying.IsEnum) return typeof(int?);
        return propertyType;
    }

    #endregion
}
