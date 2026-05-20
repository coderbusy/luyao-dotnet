using LuYao.Data.Mapping;
using LuYao.Data.Meta;
using System;

namespace LuYao.Data;

partial struct RecordRow
{
    /// <summary>
    /// 将另一行 <paramref name="other"/> 的数据合并到当前行：同名列直接覆盖，当前行不存在的列则追加。
    /// </summary>
    /// <param name="other">数据来源行，可来自不同的 <see cref="RecordTable"/>。</param>
    public void Merge(RecordRow other)
    {
        foreach (var srcCol in other.Table.Columns)
        {
            var value = srcCol.Get(other);
            var dstCol = this.Table.Columns.Find(srcCol.Name);
            if (dstCol == null)
            {
                dstCol = this.Table.Columns.Add(srcCol.Name, srcCol.Type);
            }
            dstCol.Set(this, value);
        }
    }

    /// <summary>
    /// 将对象 <paramref name="model"/> 的可读属性合并到当前行：同名列直接覆盖，当前行不存在的列则追加。
    /// 使用运行时实际类型，派生类新增属性会被正确处理。
    /// </summary>
    /// <param name="model">属性值的来源对象，不可为 null。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="model"/> 为 null 时抛出。</exception>
    public void Merge(object model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        Merge(model, RecordMappingOptions.Default);
    }

    /// <summary>
    /// 将对象 <paramref name="model"/> 的可读属性合并到当前行：同名列直接覆盖，当前行不存在的列则追加。
    /// 使用运行时实际类型，派生类新增属性会被正确处理。
    /// </summary>
    /// <param name="model">属性值的来源对象，不可为 null。</param>
    /// <param name="options">映射选项，不可为 null。</param>
    /// <exception cref="ArgumentNullException">任一参数为 null 时抛出。</exception>
    public void Merge(object model, RecordMappingOptions options)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (options == null) throw new ArgumentNullException(nameof(options));
        new RecordMappingContext(options).WriteDtoToRow(model.GetType(), model, this);
    }
}
