using LuYao.Data.Models;
using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 适配器基类
/// </summary>
public abstract class RecordSaveAdapter
{
    public abstract IReadOnlyList<RecordSection> Layout { get; }
    public abstract void WriteStart();
    public abstract void WriteEnd();
    public abstract void WriteStartSection(RecordSection section);
    public abstract void WriteEndSection();
    public abstract void WriteHeader(RecordHeader header);
    public abstract void WriteColumn(RecordColumnInfo column);
    public abstract void WriteStarRow();
    public abstract void WriteEndRow();
    public abstract void WriteBoolean(string name, int index, bool value);
    public abstract void WriteByte(string name, int index, byte value);
    public abstract void WriteChar(string name, int index, char value);
    public abstract void WriteDateTime(string name, int index, DateTime value);
    public abstract void WriteDecimal(string name, int index, decimal value);
    public abstract void WriteDouble(string name, int index, double value);
    public abstract void WriteInt16(string name, int index, short value);
    public abstract void WriteInt32(string name, int index, int value);
    public abstract void WriteInt64(string name, int index, long value);
    public abstract void WriteSByte(string name, int index, sbyte value);
    public abstract void WriteSingle(string name, int index, float value);
    public abstract void WriteString(string name, int index, string value);
    public abstract void WriteUInt16(string name, int index, ushort value);
    public abstract void WriteUInt32(string name, int index, uint value);
    public abstract void WriteUInt64(string name, int index, ulong value);
    public abstract void WriteObject(string name, int index, object? value);
}