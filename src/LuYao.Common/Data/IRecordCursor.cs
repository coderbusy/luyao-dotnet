using System;

namespace LuYao.Data;

/// <summary>
/// 记录游标接口，提供从列存储数据集合中读取各种数据类型值的功能。
/// 此接口被 <see cref="Record"/> 和 <see cref="RecordRow"/> 实现，
/// 支持通过列名或列对象访问当前游标位置的数据。
/// </summary>
interface IRecordCursor
{
    /// <summary>
    /// 根据列名获取指定泛型类型的值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到指定列则返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    T? Get<T>(string name);

    /// <summary>
    /// 根据列对象获取指定泛型类型的值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="col">要读取的列对象。</param>
    /// <returns>如果列属于当前记录则返回转换后的泛型类型值，否则返回该类型的默认值。</returns>
    T? Get<T>(RecordColumn col);
}