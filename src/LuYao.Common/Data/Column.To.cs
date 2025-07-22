using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

partial class Column
{
    ///<inheritdoc/>
    public override string ToString()
    {
        return string.Empty;
    }

    ///<inheritdoc/>
    public Int32 ToInt32(int row)
    {
        switch (this.Code)
        {
            case TypeCode.Boolean: return Valid.ToInt32(this.Data.GetValue<Boolean>(row));
            default: return default(Int32);
        }
    }
}
