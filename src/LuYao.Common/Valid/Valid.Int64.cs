using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Int64 ToInt64(object value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(DateTime value) => value.ToBinary();
}
