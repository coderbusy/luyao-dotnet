namespace LuYao.Data.Meta;

/// <summary>
/// 提供在强类型对象与 <see cref="RecordRow"/> 之间进行属性双向复制的静态工具类。
/// </summary>
/// <typeparam name="T">要映射的对象类型，必须为引用类型。</typeparam>
public static class XCopy<T> where T : class
{
    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入 <paramref name="row"/> 对应的列。
    /// </summary>
    /// <param name="data">数据来源对象。</param>
    /// <param name="row">目标行；仅写入类型受支持的可读属性。</param>
    public static void CopyTo(T data, RecordRow row)
    {
        var props = XProp.GetAll(typeof(T));
        var re = row.Record;

        foreach (var prop in props)
        {
            if (!Helpers.IsSupportedForReading(prop)) continue;
            row[prop.Name] = prop.GetValue(data);
        }
    }

    /// <summary>
    /// 将 <paramref name="row"/> 中与对象属性同名的列值写回对象 <paramref name="data"/>。
    /// </summary>
    /// <param name="data">目标对象；仅更新类型受支持且行中存在对应列的可写属性。</param>
    /// <param name="row">数据来源行。</param>
    public static void CopyFrom(T data, RecordRow row)
    {
        var props = XProp.GetAll(typeof(T));
        var re = row.Record;
        foreach (var prop in props)
        {
            if (!Helpers.IsSupportedForWriting(prop)) continue;
            if (!re.Columns.Contains(prop.Name)) continue;
            prop.SetValue(data, row[prop.Name]);
        }
    }
}