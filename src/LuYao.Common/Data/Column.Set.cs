using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

partial class Column
{
    public void Set(int value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        switch (this.Code)
        {
            case TypeCode.Int32: this.Data.SetValue(value, index); break;
        }
    }
}
