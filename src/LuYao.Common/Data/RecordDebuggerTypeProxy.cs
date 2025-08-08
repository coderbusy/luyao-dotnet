using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

internal class RecordDebuggerTypeProxy
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Record _record;

    public RecordDebuggerTypeProxy(Record record)
    {
        _record = record ?? throw new ArgumentNullException(nameof(record));
    }
    public string Name => string.IsNullOrWhiteSpace(_record.Name) ? "None" : _record.Name;
    public int Count => _record.Count;
    public string Data => _record.ToString();
}
