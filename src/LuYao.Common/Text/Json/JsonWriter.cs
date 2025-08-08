using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// ������ JSON д������ʹ�ù̶���С�������� Span �Ż�
/// </summary>
public sealed class JsonWriter : IDisposable
{
    private readonly TextWriter _writer;
    private readonly bool _ownsWriter;
    private char[]? _buffer;
    private int _bufferPosition;
    private bool _disposed;

    private const int DefaultBufferSize = 4096;

    // JSON ״̬ջ�����ڹ���Ƕ�׽ṹ
    private JsonWriteState[] _stateStack;
    private int _stackDepth;

    // ��ʽ��ѡ��
    private readonly bool _indented;
    private readonly string _indentString;
    private int _currentIndentLevel;

    /// <summary>
    /// ��ʼ�� JsonWriter ʵ��
    /// </summary>
    /// <param name="writer">Ҫд��� TextWriter</param>
    /// <param name="ownsWriter">�Ƿ�ӵ�в��ͷ� TextWriter</param>
    /// <param name="indented">�Ƿ�����������ʽ��</param>
    /// <param name="indentString">�����ַ���</param>
    /// <param name="bufferSize">��������С</param>
    public JsonWriter(TextWriter writer, bool ownsWriter = false, bool indented = false,
                     string indentString = "  ", int bufferSize = DefaultBufferSize)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _ownsWriter = ownsWriter;
        _indented = indented;
        _indentString = indentString ?? "  ";
        _buffer = new char[Math.Max(bufferSize, 64)];
        _stateStack = new JsonWriteState[8]; // ��ʼջ��С
    }

    /// <summary>
    /// ʹ�� StringBuilder ��ʼ�� JsonWriter ʵ��
    /// </summary>
    /// <param name="sb">Ҫд��� StringBuilder</param>
    /// <param name="indented">�Ƿ�����������ʽ��</param>
    /// <param name="indentString">�����ַ���</param>
    public JsonWriter(StringBuilder sb, bool indented = false, string indentString = "  ")
        : this(new StringWriter(sb), true, indented, indentString)
    {
    }

    private enum JsonWriteState
    {
        Start,
        Object,
        Array,
        Property,
        Value
    }

    /// <summary>
    /// д�����ʼ�� {
    /// </summary>
    public void WriteStartObject()
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteChar('{');
        PushState(JsonWriteState.Object);
        IncreaseIndent();
        if (_indented) WriteNewLine(); // ����������������ٻ���
    }

    /// <summary>
    /// д���������� }
    /// </summary>
    public void WriteEndObject()
    {
        PopState(JsonWriteState.Object);
        DecreaseIndent();
        if (_indented) WriteNewLine(); // �Ȼ�����д����
        WriteIndentIfNeeded();
        WriteChar('}');
    }

    /// <summary>
    /// д�����鿪ʼ�� [
    /// </summary>
    public void WriteStartArray()
    {
        WriteValueSeparator();
        WriteChar('[');
        PushState(JsonWriteState.Array);
        IncreaseIndent();
        if (_indented) WriteNewLine(); // ����������������ٻ���
    }

    /// <summary>
    /// д����������� ]
    /// </summary>
    public void WriteEndArray()
    {
        PopState(JsonWriteState.Array);
        DecreaseIndent();
        if (_indented) WriteNewLine(); // �Ȼ�����д����
        WriteIndentIfNeeded();
        WriteChar(']');
    }

    /// <summary>
    /// д��������
    /// </summary>
    public void WritePropertyName(string? name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteStringEscaped(name);
        WriteChar(':');
        if (_indented) WriteChar(' ');

        SetState(JsonWriteState.Property);
    }

    /// <summary>
    /// д���ַ���ֵ
    /// </summary>
    public void WriteValue(string? value)
    {
        WriteValueSeparator();

        if (value == null)
        {
            WriteNull();
        }
        else
        {
            WriteStringEscaped(value);
        }

        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// д������ֵ
    /// </summary>
    public void WriteValue(long value)
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString(value.ToString(CultureInfo.InvariantCulture));
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// д�븡��ֵ
    /// </summary>
    public void WriteValue(double value)
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();

        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            WriteNull();
        }
        else
        {
            WriteString(value.ToString("R", CultureInfo.InvariantCulture));
        }

        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// д�벼��ֵ
    /// </summary>
    public void WriteValue(bool value)
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString(value ? "true" : "false");
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// д�� null ֵ
    /// </summary>
    public void WriteNull()
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString("null");
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// д��ԭʼ JSON
    /// </summary>
    public void WriteRaw(string? json)
    {
        if (json == null)
            throw new ArgumentNullException(nameof(json));

        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString(json);
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// ˢ�»�����
    /// </summary>
    public void Flush()
    {
        if (_bufferPosition > 0)
        {
            _writer.Write(_buffer, 0, _bufferPosition);
            _bufferPosition = 0;
        }
        _writer.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteChar(char c)
    {
        EnsureCapacity(1);
        _buffer![_bufferPosition++] = c;
    }

    /// <summary>
    /// д��ԭʼ�ַ�����������ת��
    /// </summary>
    private void WriteString(string? str)
    {
        if (str == null) return;

        EnsureCapacity(str.Length);
        for (int i = 0; i < str.Length; i++)
        {
            _buffer![_bufferPosition++] = str[i];
        }
    }

    /// <summary>
    /// д���ַ���������JSONת��
    /// </summary>
    private void WriteStringEscaped(string value)
    {
        if (value == null) return;

        WriteChar('"');

        foreach (char c in value)
        {
            switch (c)
            {
                case '"':
                    WriteString("\\\"");
                    break;
                case '\\':
                    WriteString("\\\\");
                    break;
                case '\b':
                    WriteString("\\b");
                    break;
                case '\f':
                    WriteString("\\f");
                    break;
                case '\n':
                    WriteString("\\n");
                    break;
                case '\r':
                    WriteString("\\r");
                    break;
                case '\t':
                    WriteString("\\t");
                    break;
                default:
                    if (char.IsControl(c))
                    {
                        WriteString($"\\u{(int)c:x4}");
                    }
                    else
                    {
                        WriteChar(c);
                    }
                    break;
            }
        }

        WriteChar('"');
    }

    private void EnsureCapacity(int count)
    {
        if (_bufferPosition + count >= _buffer!.Length)
        {
            Flush();
        }
    }

    private void WriteValueSeparator()
    {
        if (_stackDepth > 0)
        {
            var currentState = _stateStack[_stackDepth - 1];
            if (currentState == JsonWriteState.Array || currentState == JsonWriteState.Object)
            {
                // ��һ��Ԫ�ز���Ҫ����
                _stateStack[_stackDepth - 1] = JsonWriteState.Value;
            }
            else if (currentState == JsonWriteState.Value)
            {
                WriteChar(',');
                if (_indented) WriteNewLine();
            }
            // Property ״̬����Ҫ���ţ�ֻ��Ҫ����Ϊ Value
            else if (currentState == JsonWriteState.Property)
            {
                // �����������ֵ������Ҫ����
            }
        }
    }

    private void WriteIndentIfNeeded()
    {
        if (_indented && _currentIndentLevel > 0)
        {
            for (int i = 0; i < _currentIndentLevel; i++)
            {
                WriteString(_indentString);
            }
        }
    }

    private void WriteNewLine()
    {
        WriteChar('\n');
    }

    private void PushState(JsonWriteState state)
    {
        if (_stackDepth >= _stateStack.Length)
        {
            Array.Resize(ref _stateStack, _stateStack.Length * 2);
        }
        _stateStack[_stackDepth++] = state;
    }

    private void PopState(JsonWriteState expectedState)
    {
        if (_stackDepth == 0)
            throw new InvalidOperationException("Invalid JSON structure: unexpected end marker");

        var currentState = _stateStack[--_stackDepth];
        // ����� Value �� Property ״̬�˳� Object/Array����Ϊ��Щ������Ч��״̬
        if (currentState != expectedState && currentState != JsonWriteState.Value &&
            !(expectedState == JsonWriteState.Object && currentState == JsonWriteState.Property))
            throw new InvalidOperationException($"Invalid JSON structure: expected {expectedState}, got {currentState}");
    }

    private void SetState(JsonWriteState state)
    {
        if (_stackDepth > 0)
        {
            _stateStack[_stackDepth - 1] = state;
        }
    }

    private void IncreaseIndent()
    {
        if (_indented)
        {
            _currentIndentLevel++;
        }
    }

    private void DecreaseIndent()
    {
        if (_indented)
        {
            _currentIndentLevel--;
        }
    }

    /// <summary>
    /// �ͷ���Դ
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Flush();

            _buffer = null;

            if (_ownsWriter)
            {
                _writer?.Dispose();
            }

            _disposed = true;
        }
    }
}