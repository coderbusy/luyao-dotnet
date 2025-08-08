using System;

namespace LuYao.Data.Models;

/// <summary>
/// 表示记录的模式信息。
/// </summary>
public class RecordHeader
{
    /// <summary>
    /// 初始化 <see cref="RecordHeader"/> 类的新实例。
    /// </summary>
    public RecordHeader()
    {

    }

    /// <summary>
    /// 使用指定的记录初始化 <see cref="RecordHeader"/> 类的新实例。
    /// </summary>
    /// <param name="re">记录对象。</param>
    public RecordHeader(Record re)
    {
        if (re == null) throw new ArgumentNullException(nameof(re));
        Name = re.Name;
        Columns = re.Columns.Count;
        Count = re.Count;
    }

    /// <summary>
    /// 获取或设置记录的名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置列的数量。
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// 获取或设置记录的数量。
    /// </summary>
    public int Count { get; set; }
}
