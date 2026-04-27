using System;
using System.Diagnostics;

namespace LuYao.Data;

internal class FrameDebuggerTypeProxy
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Frame _record;

    public FrameDebuggerTypeProxy(Frame record)
    {
        _record = record ?? throw new ArgumentNullException(nameof(record));
    }
    public string Name => string.IsNullOrWhiteSpace(_record.Name) ? "None" : _record.Name;
    public int Count => _record.Count;
    public string Data => _record.ToString();
}
