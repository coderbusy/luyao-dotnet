using System;

namespace LuYao.Data.Models;

public class RecordColumnInfo
{
    public RecordColumnInfo()
    {

    }
    public RecordColumnInfo(RecordColumn column) : this()
    {
        this.Name = column.Name;
        this.Code = column.Code;
        this.Type = column.Type;
    }
    public string Name { get; set; }
    public RecordDataCode Code { get; set; }
    public Type Type { get; set; }
}
