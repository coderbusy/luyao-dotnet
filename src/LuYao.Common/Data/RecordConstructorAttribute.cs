using System;

namespace LuYao.Data;

/// <summary>
/// 标记用于对象映射的构造函数。
/// 当类型标记了此特性时，映射层将使用该构造函数创建对象，按参数名匹配列名。
/// 每个类型最多只能标记一个构造函数，否则抛出 <see cref="System.Reflection.AmbiguousMatchException"/>。
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class RecordConstructorAttribute : Attribute
{
}
