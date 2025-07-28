using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 记录列名特性，用于指定属性对应的数据库列名
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class RecordColumnNameAttribute : Attribute
{
    /// <summary>
    /// 初始化记录列名特性的新实例
    /// </summary>
    /// <param name="name">数据库列名</param>
    /// <exception cref="ArgumentException">当列名为空或空白时抛出异常</exception>
    public RecordColumnNameAttribute(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("列名不能为空或空白", nameof(name));
        Name = name;
    }

    /// <summary>
    /// 获取数据库列名
    /// </summary>
    public string Name { get; }
}
