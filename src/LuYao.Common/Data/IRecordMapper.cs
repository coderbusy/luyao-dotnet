namespace LuYao.Data;

/// <summary>
/// 自定义对象映射器（非泛型基接口）。
/// 当设置到 <see cref="RecordMappingOptions.Mapper"/> 时，映射层将所有工作委托给它，
/// 不再进行反射、名称匹配或类型转换。
/// </summary>
public interface IRecordMapper
{
    /// <summary>
    /// 将对象的值写入 Record 的指定行。
    /// </summary>
    /// <param name="item">要写入的对象。</param>
    /// <param name="record">目标 Record。</param>
    /// <param name="row">目标行索引。</param>
    void Write(object item, Record record, int row);

    /// <summary>
    /// 从 Record 的指定行创建对象。
    /// </summary>
    /// <param name="record">源 Record。</param>
    /// <param name="row">源行索引。</param>
    /// <returns>创建的对象。</returns>
    object Read(Record record, int row);
}

/// <summary>
/// 自定义对象映射器（泛型接口），提供强类型支持。
/// </summary>
/// <typeparam name="T">映射的目标类型。</typeparam>
public interface IRecordMapper<T> : IRecordMapper
{
    /// <summary>
    /// 将对象的值写入 Record 的指定行。
    /// </summary>
    /// <param name="item">要写入的对象。</param>
    /// <param name="record">目标 Record。</param>
    /// <param name="row">目标行索引。</param>
    void Write(T item, Record record, int row);

    /// <summary>
    /// 从 Record 的指定行创建对象。
    /// </summary>
    /// <param name="record">源 Record。</param>
    /// <param name="row">源行索引。</param>
    /// <returns>创建的对象。</returns>
    new T Read(Record record, int row);
}
