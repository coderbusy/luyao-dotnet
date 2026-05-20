using System;
using System.Collections.Generic;
using System.Globalization;

namespace LuYao.Data.Meta;

/// <summary>
/// 内置托底转换器，支持常见数值类型、<see cref="string"/> 之间的互转。
/// 优先级最低，仅在 <see cref="RecordMappingOptions"/> 中无匹配转换器时使用。
/// </summary>
internal sealed class DefaultRecordConverter : RecordConverter
{
    /// <summary>全局单例。</summary>
    public static readonly DefaultRecordConverter Instance = new DefaultRecordConverter();

    private DefaultRecordConverter() { }

    // 支持的类型对（双向）：key 为小类型，value 为大类型；查找时双向均可命中
    private static readonly HashSet<TypePair> _supported = BuildSupportedPairs();

    private static HashSet<TypePair> BuildSupportedPairs()
    {
        var numericTypes = new[]
        {
            typeof(byte),    typeof(sbyte),
            typeof(short),   typeof(ushort),
            typeof(int),     typeof(uint),
            typeof(long),    typeof(ulong),
            typeof(float),   typeof(double),
            typeof(decimal),
        };

        var set = new HashSet<TypePair>();

        // 数值类型两两互转
        for (int i = 0; i < numericTypes.Length; i++)
        {
            for (int j = 0; j < numericTypes.Length; j++)
            {
                if (i != j)
                    set.Add(new TypePair(numericTypes[i], numericTypes[j]));
            }
        }

        // 数值类型 ↔ string
        foreach (var t in numericTypes)
        {
            set.Add(new TypePair(t, typeof(string)));
            set.Add(new TypePair(typeof(string), t));
        }

        // bool ↔ string
        set.Add(new TypePair(typeof(bool), typeof(string)));
        set.Add(new TypePair(typeof(string), typeof(bool)));

        // bool ↔ 数值类型（0/1）
        foreach (var t in numericTypes)
        {
            set.Add(new TypePair(typeof(bool), t));
            set.Add(new TypePair(t, typeof(bool)));
        }

        // DateTime ↔ string
        set.Add(new TypePair(typeof(DateTime), typeof(string)));
        set.Add(new TypePair(typeof(string), typeof(DateTime)));

        // char ↔ string
        set.Add(new TypePair(typeof(char), typeof(string)));
        set.Add(new TypePair(typeof(string), typeof(char)));

        return set;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type sourceType, Type targetType)
    {
        if (sourceType == null || targetType == null) return false;
        if (sourceType == targetType) return true;
        return _supported.Contains(new TypePair(sourceType, targetType));
    }

    /// <inheritdoc/>
    public override object? Convert(Type sourceType, Type targetType, object? value)
    {
        if (!CanConvert(sourceType, targetType))
            throw new NotSupportedException(
                $"DefaultRecordConverter 不支持从 {sourceType.Name} 到 {targetType.Name} 的转换。");

        if (value == null)
        {
            // 值类型返回默认值
            if (targetType.IsValueType)
                return Activator.CreateInstance(targetType);
            return null;
        }

        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }

    private readonly struct TypePair : IEquatable<TypePair>
    {
        public readonly Type Source;
        public readonly Type Target;

        public TypePair(Type source, Type target)
        {
            Source = source;
            Target = target;
        }

        public bool Equals(TypePair other) => Source == other.Source && Target == other.Target;
        public override bool Equals(object? obj) => obj is TypePair other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                return (Source.GetHashCode() * 397) ^ Target.GetHashCode();
            }
        }
    }
}
