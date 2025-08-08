using LuYao.Data.Models;
using System;

namespace LuYao.Data;

public abstract class RecordLoadAdapter
{
    public abstract RecordLoadKeyKind KeyKind { get; }
    public abstract int Index { get; }
    public abstract string Name { get; }
    public abstract RecordHeader ReadHeader();
    public abstract RecordColumnInfo ReadColumn();
    public abstract bool ReadRow();
    public abstract bool ReadField();
    public abstract bool ReadBoolean();
    public abstract byte ReadByte();
    public abstract char ReadChar();
    public abstract DateTime ReadDateTime();
    public abstract decimal ReadDecimal();
    public abstract double ReadDouble();
    public abstract short ReadInt16();
    public abstract int ReadInt32();
    public abstract long ReadInt64();
    public abstract sbyte ReadSByte();
    public abstract float ReadSingle();
    public abstract string ReadString();
    public abstract ushort ReadUInt16();
    public abstract uint ReadUInt32();
    public abstract ulong ReadUInt64();
    public abstract object? ReadObject(object type);
}