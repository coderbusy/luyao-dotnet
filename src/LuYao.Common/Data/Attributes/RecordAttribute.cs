using System;

namespace LuYao.Data.Attributes;

/// <summary>
/// 用于标记 Record 映射行为的特性基类。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class RecordAttribute : Attribute
{
}
