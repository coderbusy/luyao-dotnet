using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LuYao.Text.Json;

/// <summary>
/// 高性能 JSON 写入器，使用固定大小缓冲区和 Span 优化
/// </summary>
public sealed class JsonWriter : IDisposable
{
    private readonly TextWriter _writer;
    private readonly bool _ownsWriter;
    private char[]? _buffer;
    private int _bufferPosition;
    private bool _disposed;

    private const int DefaultBufferSize = 4096;

    // JSON 状态栈，用于管理嵌套结构
    private JsonWriteState[] _stateStack;
    private int _stackDepth;

    // 格式化选项
    private readonly bool _indented;
    private readonly string _indentString;
    private int _currentIndentLevel;

    /// <summary>
    /// 初始化 JsonWriter 实例
    /// </summary>
    /// <param name="writer">要写入的 TextWriter</param>
    /// <param name="ownsWriter">是否拥有并释放 TextWriter</param>
    /// <param name="indented">是否启用缩进格式化</param>
    /// <param name="indentString">缩进字符串</param>
    /// <param name="bufferSize">缓冲区大小</param>
    public JsonWriter(TextWriter writer, bool ownsWriter = false, bool indented = false,
                     string indentString = "  ", int bufferSize = DefaultBufferSize)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _ownsWriter = ownsWriter;
        _indented = indented;
        _indentString = indentString ?? "  ";
        _buffer = new char[Math.Max(bufferSize, 64)];
        _stateStack = new JsonWriteState[8]; // 初始栈大小
    }

    /// <summary>
    /// 使用 StringBuilder 初始化 JsonWriter 实例
    /// </summary>
    /// <param name="sb">要写入的 StringBuilder</param>
    /// <param name="indented">是否启用缩进格式化</param>
    /// <param name="indentString">缩进字符串</param>
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
    /// 写入对象开始符 {
    /// </summary>
    public void WriteStartObject()
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteChar('{');
        PushState(JsonWriteState.Object);
        IncreaseIndent();
        if (_indented) WriteNewLine(); // 在增加缩进级别后再换行
    }

    /// <summary>
    /// 写入对象结束符 }
    /// </summary>
    public void WriteEndObject()
    {
        PopState(JsonWriteState.Object);
        DecreaseIndent();
        if (_indented) WriteNewLine(); // 先换行再写缩进
        WriteIndentIfNeeded();
        WriteChar('}');
    }

    /// <summary>
    /// 写入数组开始符 [
    /// </summary>
    public void WriteStartArray()
    {
        WriteValueSeparator();
        WriteChar('[');
        PushState(JsonWriteState.Array);
        IncreaseIndent();
        if (_indented) WriteNewLine(); // 在增加缩进级别后再换行
    }

    /// <summary>
    /// 写入数组结束符 ]
    /// </summary>
    public void WriteEndArray()
    {
        PopState(JsonWriteState.Array);
        DecreaseIndent();
        if (_indented) WriteNewLine(); // 先换行再写缩进
        WriteIndentIfNeeded();
        WriteChar(']');
    }

    /// <summary>
    /// 写入属性名
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
    /// 写入字符串值
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
    /// 写入整数值
    /// </summary>
    public void WriteValue(long value)
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString(value.ToString(CultureInfo.InvariantCulture));
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// 写入浮点值
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
    /// 写入布尔值
    /// </summary>
    public void WriteValue(bool value)
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString(value ? "true" : "false");
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// 写入 null 值
    /// </summary>
    public void WriteNull()
    {
        WriteValueSeparator();
        WriteIndentIfNeeded();
        WriteString("null");
        SetState(JsonWriteState.Value);
    }

    /// <summary>
    /// 写入原始 JSON
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
    /// 刷新缓冲区
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
    /// 写入原始字符串，不进行转义
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
    /// 写入字符串，进行JSON转义
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
                // 第一个元素不需要逗号
                _stateStack[_stackDepth - 1] = JsonWriteState.Value;
            }
            else if (currentState == JsonWriteState.Value)
            {
                WriteChar(',');
                if (_indented) WriteNewLine();
            }
            // Property 状态不需要逗号，只需要设置为 Value
            else if (currentState == JsonWriteState.Property)
            {
                // 属性名后面跟值，不需要逗号
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
        // 允许从 Value 或 Property 状态退出 Object/Array，因为这些都是有效的状态
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
    /// 释放资源
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