using System;

namespace LuYao.Data.Mapping;

public abstract class XConverter
{
    protected XConverter(Type sourceType, Type targetType)
    {
        SourceType = sourceType;
        TargetType = targetType;
    }
    public Type SourceType { get; }
    public Type TargetType { get; }
    public abstract Func<object?, object?> ConvertToSource { get; }
    public abstract Func<object?, object?> ConvertFromSource { get; }
}
