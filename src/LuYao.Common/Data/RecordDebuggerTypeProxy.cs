using System;
using System.Diagnostics;

namespace LuYao.Data;

internal class RecordTableDebuggerTypeProxy
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly RecordTable _table;

    public RecordTableDebuggerTypeProxy(RecordTable table)
    {
        _table = table ?? throw new ArgumentNullException(nameof(table));
    }
    public string Name => string.IsNullOrWhiteSpace(_table.Name) ? "None" : _table.Name;
    public int Count => _table.Count;
    public string Data => _table.ToString();
}
