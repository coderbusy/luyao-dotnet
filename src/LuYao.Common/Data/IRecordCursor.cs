using System;

namespace LuYao.Data;

interface IRecordCursor
{
    Boolean GetBoolean(string name);
    Boolean GetBoolean(RecordColumn col);

    Byte GetByte(string name);
    Byte GetByte(RecordColumn col);

    Char GetChar(string name);
    Char GetChar(RecordColumn col);

    DateTime GetDateTime(string name);
    DateTime GetDateTime(RecordColumn col);

    Decimal GetDecimal(string name);
    Decimal GetDecimal(RecordColumn col);

    Double GetDouble(string name);
    Double GetDouble(RecordColumn col);

    Int16 GetInt16(string name);
    Int16 GetInt16(RecordColumn col);

    Int32 GetInt32(string name);
    Int32 GetInt32(RecordColumn col);

    Int64 GetInt64(string name);
    Int64 GetInt64(RecordColumn col);

    SByte GetSByte(string name);
    SByte GetSByte(RecordColumn col);

    Single GetSingle(string name);
    Single GetSingle(RecordColumn col);

    String? GetString(string name);
    String? GetString(RecordColumn col);

    UInt16 GetUInt16(string name);
    UInt16 GetUInt16(RecordColumn col);

    UInt32 GetUInt32(string name);
    UInt32 GetUInt32(RecordColumn col);

    UInt64 GetUInt64(string name);
    UInt64 GetUInt64(RecordColumn col);

    T? Get<T>(string name);
    T? Get<T>(RecordColumn col);
}
