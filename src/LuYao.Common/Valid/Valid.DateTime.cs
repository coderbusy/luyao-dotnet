using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static DateTime ToDateTime(long value) => DateTime.FromBinary(value);
}
