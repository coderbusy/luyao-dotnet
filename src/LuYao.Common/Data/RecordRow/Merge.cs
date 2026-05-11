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
    /// </summary>
    /// <typeparam name="T">数据来源对象类型。</typeparam>
    /// <param name="model">属性值的来源对象。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="model"/> 为 null 时抛出。</exception>
    public void Merge<T>(T model) where T : class
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        var props = XProp.GetAll(typeof(T));
        foreach (var prop in props)
        {
            if (!prop.CanRead) continue;
            if (!Helpers.IsSupportedForReading(prop)) continue;
            var value = prop.GetValue(model);
            var col = this.Table.Columns.Find(prop.Name);
            if (col == null)
            {
                col = this.Table.Columns.Add(prop.Name, prop.Type);
            }
            col.Set(this, value);
        }
    }
}
