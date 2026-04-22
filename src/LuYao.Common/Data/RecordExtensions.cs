using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

public static class RecordExtensions
{
    public static Dictionary<T, List<RecordRow>> Group<T>(this Record re, string fld) where T : notnull
    {
        var ret = new Dictionary<T, List<RecordRow>>();
        var col = re.Columns.Find(fld);
        if (col == null)
        {
            ret.Add(default!, re.ToList());
            return ret;
        }
        foreach (var row in re)
        {
            T key = col.To<T>(row)!;
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }
}
