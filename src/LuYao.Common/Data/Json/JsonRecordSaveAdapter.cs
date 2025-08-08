using LuYao.Data.Models;
using LuYao.Text.Json;
using System;
using System.Collections.Generic;

namespace LuYao.Data.Json;

/// <summary>
/// �ṩ����¼������JSON��ʽд������������ʵ�֡�
/// </summary>
/// <remarks>
/// ����̳��� <see cref="RecordSaveAdapter"/>��ר�����ڽ���¼�������л�ΪJSON��ʽ��
/// ��ʹ�� <see cref="JsonWriter"/> ��������������д��ײ����С�
/// JSON�ṹ����header��columns��rows�Ȳ��֡�
/// </remarks>
public class JsonRecordSaveAdapter : RecordSaveAdapter
{
    /// <summary>
    /// ��ȡ����д��JSON���ݵ� <see cref="JsonWriter"/> ʵ����
    /// </summary>
    /// <value>����ִ��JSONд������� <see cref="JsonWriter"/> ����</value>
    public JsonWriter Writer { get; }

    /// <inheritdoc/>
    public override IReadOnlyList<RecordSection> Layout { get; } = [RecordSection.Head, RecordSection.Columns, RecordSection.Rows];

    /// <summary>
    /// ʹ��ָ����JSONд������ʼ�� <see cref="JsonRecordSaveAdapter"/> �����ʵ����
    /// </summary>
    /// <param name="writer">����д��JSON���ݵ� <see cref="JsonWriter"/> ʵ����</param>
    /// <exception cref="ArgumentNullException">�� <paramref name="writer"/> Ϊ null ʱ�׳���</exception>
    public JsonRecordSaveAdapter(JsonWriter writer)
    {
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    /// <remarks>
    /// ������Ϣд��JSON����Ϊ���󲢽����Դ洢Ϊ��ֵ�ԡ�
    /// </remarks>
    public override void WriteColumn(RecordColumnInfo column)
    {
        string type = column.GetTypeName();
        Writer.WriteStartObject();
        Writer.WritePropertyName("name");
        Writer.WriteValue(column.Name);
        Writer.WritePropertyName("code");
        Writer.WriteValue(Valid.ToString(column.Code));
        Writer.WritePropertyName("type");
        Writer.WriteValue(type);
        Writer.WriteEndObject();
    }

    /// <remarks>
    /// ����¼ͷ��Ϣд��JSON����Ϊ���󲢽����Դ洢Ϊ��ֵ�ԡ�
    /// </remarks>
    public override void WriteHeader(RecordHeader header)
    {
        Writer.WriteStartObject();
        Writer.WritePropertyName("name");
        Writer.WriteValue(header.Name);
        Writer.WritePropertyName("columns");
        Writer.WriteValue(header.Columns);
        Writer.WritePropertyName("count");
        Writer.WriteValue(header.Count);
        Writer.WriteEndObject();
    }

    /// <exception cref="NotImplementedException">�������͵�JSON��д�ݲ�֧�֡�</exception>
    public override void WriteObject(string name, int index, object? value) => throw new NotImplementedException("�������͵�JSON��д�ݲ�֧��");

    /// <remarks>
    /// ������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteBoolean(string name, int index, bool value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// ���ֽ�ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteByte(string name, int index, byte value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// ���ַ�ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteChar(string name, int index, char value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(Valid.ToString(value));
    }

    /// <remarks>
    /// ������ʱ��ֵת��ΪISO 8601��ʽ����Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteDateTime(string name, int index, DateTime value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value.ToString("O"));
    }

    /// <remarks>
    /// ��ʮ������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteDecimal(string name, int index, decimal value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((double)value);
    }

    /// <remarks>
    /// ��˫���ȸ�����ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteDouble(string name, int index, double value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// ������ǰ�ж����д�롣
    /// </remarks>
    public override void WriteEndRow() => Writer.WriteEndObject();

    /// <remarks>
    /// ��16λ�з�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteInt16(string name, int index, short value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// ��32λ�з�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteInt32(string name, int index, int value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// ��64λ�з�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteInt64(string name, int index, long value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// ��8λ�з�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteSByte(string name, int index, sbyte value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// �������ȸ�����ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteSingle(string name, int index, float value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((double)value);
    }

    /// <remarks>
    /// ��ʼд���µ��ж���
    /// </remarks>
    public override void WriteStarRow() => Writer.WriteStartObject();

    /// <remarks>
    /// ���ַ���ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteString(string name, int index, string value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue(value);
    }

    /// <remarks>
    /// ��16λ�޷�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteUInt16(string name, int index, ushort value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// ��32λ�޷�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteUInt32(string name, int index, uint value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <remarks>
    /// ��64λ�޷�������ֵ��Ϊ����д�뵱ǰ�ж���
    /// </remarks>
    public override void WriteUInt64(string name, int index, ulong value)
    {
        Writer.WritePropertyName(name);
        Writer.WriteValue((long)value);
    }

    /// <inheritdoc/>
    public override void WriteStart()
    {
        Writer.WriteStartObject();
    }

    /// <inheritdoc/>
    public override void WriteEnd()
    {
        Writer.WriteEndObject();
    }

    /// <inheritdoc/>
    public override void WriteStartSection(RecordSection section)
    {
        Writer.WritePropertyName(section.ToString().ToLowerInvariant());
        Writer.WriteStartArray();
    }

    /// <inheritdoc/>
    public override void WriteEndSection()
    {
        Writer.WriteEndArray();
    }
}