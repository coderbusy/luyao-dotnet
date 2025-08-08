using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LuYao.Text.Json;

/// <summary>
/// ������ JSON ��ȡ����ʹ�ù̶���С�������Ż�����
/// </summary>
public sealed class JsonReader : IDisposable
{
    private readonly TextReader _reader;
    private readonly bool _ownsReader;
    private char[] _buffer;
    private int _bufferPosition;
    private int _bufferLength;
    private int _currentIndex;
    private int _line = 1;
    private int _column = 1;
    private bool _disposed;

    private const int DefaultBufferSize = 4096;

    public JsonReader(TextReader reader, bool ownsReader = false, int bufferSize = DefaultBufferSize)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _ownsReader = ownsReader;
        _buffer = new char[Math.Max(bufferSize, 64)];
    }

    public JsonReader(string json) : this(new StringReader(json), true)
    {
    }

    /// <summary>
    /// ��ǰ�������
    /// </summary>
    public JsonTokenType TokenType { get; private set; } = JsonTokenType.None;

    /// <summary>
    /// ��ǰ���ֵ
    /// </summary>
    public object? Value { get; private set; }

    /// <summary>
    /// ��ǰ�к�
    /// </summary>
    public int Line => _line;

    /// <summary>
    /// ��ǰ�к�
    /// </summary>
    public int Column => _column;

    /// <summary>
    /// ��ȡ��һ�����
    /// </summary>
    /// <returns>����ɹ���ȡ������򷵻� true�����򷵻� false</returns>
    public bool Read()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(JsonReader));

        SkipWhitespace();

        if (!EnsureBuffer())
        {
            TokenType = JsonTokenType.None;
            Value = null;
            return false;
        }

        char c = _buffer[_bufferPosition];

        switch (c)
        {
            case '{':
                TokenType = JsonTokenType.StartObject;
                Value = null;
                ConsumeChar();
                return true;

            case '}':
                TokenType = JsonTokenType.EndObject;
                Value = null;
                ConsumeChar();
                return true;

            case '[':
                TokenType = JsonTokenType.StartArray;
                Value = null;
                ConsumeChar();
                return true;

            case ']':
                TokenType = JsonTokenType.EndArray;
                Value = null;
                ConsumeChar();
                return true;

            case '"':
                return ReadString();

            case 't':
            case 'f':
                return ReadBoolean();

            case 'n':
                return ReadNull();

            case ',':
            case ':':
                ConsumeChar();
                return Read(); // �����ָ�������ȡ��һ�����

            default:
                if (IsDigit(c) || c == '-' || c == '+')
                    return ReadNumber();

                throw new JsonException($"Unexpected character '{c}' at line {_line}, column {_column}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SkipWhitespace()
    {
        while (EnsureBuffer())
        {
            char c = _buffer[_bufferPosition];
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
            {
                ConsumeChar();
            }
            else
            {
                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ConsumeChar()
    {
        if (_buffer[_bufferPosition] == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        _bufferPosition++;
        _currentIndex++;
    }

    private bool EnsureBuffer()
    {
        if (_bufferPosition >= _bufferLength)
        {
            _bufferLength = _reader.Read(_buffer, 0, _buffer.Length);
            _bufferPosition = 0;

            if (_bufferLength == 0)
                return false;
        }
        return true;
    }

    private bool ReadString()
    {
        ConsumeChar(); // ���ѿ�ʼ������

        var sb = new StringBuilder();

        while (EnsureBuffer())
        {
            char c = _buffer[_bufferPosition];

            if (c == '"')
            {
                ConsumeChar(); // ���ѽ���������

                // �ж��Ƿ�Ϊ�����������жϣ��鿴��һ���ǿհ��ַ��Ƿ�Ϊð�ţ�
                int savedPos = _bufferPosition;
                int savedLine = _line;
                int savedColumn = _column;

                SkipWhitespace();
                bool isPropertyName = EnsureBuffer() && _buffer[_bufferPosition] == ':';

                // �ָ�λ�ú����к�
                _bufferPosition = savedPos;
                _line = savedLine;
                _column = savedColumn;

                TokenType = isPropertyName ? JsonTokenType.PropertyName : JsonTokenType.String;
                Value = sb.ToString();
                return true;
            }
            else if (c == '\\')
            {
                ConsumeChar();
                if (!EnsureBuffer())
                    throw new JsonException("Unexpected end of input while reading string escape sequence");

                char escaped = _buffer[_bufferPosition];
                ConsumeChar();

                switch (escaped)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case '/': sb.Append('/'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u':
                        // Unicode ת������ \uXXXX
                        string hexDigits = ReadHexDigits(4);
                        if (int.TryParse(hexDigits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int unicode))
                        {
                            sb.Append((char)unicode);
                        }
                        else
                        {
                            throw new JsonException($"Invalid unicode escape sequence \\u{hexDigits}");
                        }
                        break;
                    default:
                        throw new JsonException($"Invalid escape sequence \\{escaped}");
                }
            }
            else if (char.IsControl(c))
            {
                throw new JsonException($"Unescaped control character in string at line {_line}, column {_column}");
            }
            else
            {
                sb.Append(c);
                ConsumeChar();
            }
        }

        throw new JsonException("Unexpected end of input while reading string");
    }

    private string ReadHexDigits(int count)
    {
        var sb = new StringBuilder(count);
        for (int i = 0; i < count; i++)
        {
            if (!EnsureBuffer())
                throw new JsonException("Unexpected end of input while reading unicode escape sequence");

            char c = _buffer[_bufferPosition];
            if (!IsHexDigit(c))
                throw new JsonException($"Invalid hex digit '{c}' in unicode escape sequence");

            sb.Append(c);
            ConsumeChar();
        }
        return sb.ToString();
    }

    private bool ReadNumber()
    {
        var sb = new StringBuilder();
        bool hasDecimalPoint = false;
        bool hasExponent = false;

        // ������
        if (EnsureBuffer() && (_buffer[_bufferPosition] == '-' || _buffer[_bufferPosition] == '+'))
        {
            sb.Append(_buffer[_bufferPosition]);
            ConsumeChar();
        }

        // ��ȡ���ֲ���
        bool hasDigits = false;
        while (EnsureBuffer())
        {
            char c = _buffer[_bufferPosition];

            if (IsDigit(c))
            {
                sb.Append(c);
                hasDigits = true;
                ConsumeChar();
            }
            else if (c == '.' && !hasDecimalPoint && !hasExponent)
            {
                hasDecimalPoint = true;
                sb.Append(c);
                ConsumeChar();
            }
            else if ((c == 'e' || c == 'E') && !hasExponent && hasDigits)
            {
                hasExponent = true;
                sb.Append(c);
                ConsumeChar();

                // ָ�����ֿ����з���
                if (EnsureBuffer() && (_buffer[_bufferPosition] == '+' || _buffer[_bufferPosition] == '-'))
                {
                    sb.Append(_buffer[_bufferPosition]);
                    ConsumeChar();
                }
            }
            else
            {
                break;
            }
        }

        if (!hasDigits)
            throw new JsonException($"Invalid number format at line {_line}, column {_column}");

        string numberStr = sb.ToString();
        TokenType = JsonTokenType.Number;

        // ���Խ���Ϊ��ͬ����������
        if (!hasDecimalPoint && !hasExponent)
        {
            if (long.TryParse(numberStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue))
            {
                Value = longValue;
                return true;
            }
        }

        if (double.TryParse(numberStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
        {
            Value = doubleValue;
            return true;
        }

        throw new JsonException($"Invalid number format '{numberStr}' at line {_line}, column {_column}");
    }

    private bool ReadBoolean()
    {
        if (ReadLiteral("true"))
        {
            TokenType = JsonTokenType.Boolean;
            Value = true;
            return true;
        }

        if (ReadLiteral("false"))
        {
            TokenType = JsonTokenType.Boolean;
            Value = false;
            return true;
        }

        throw new JsonException($"Invalid boolean value at line {_line}, column {_column}");
    }

    private bool ReadNull()
    {
        if (ReadLiteral("null"))
        {
            TokenType = JsonTokenType.Null;
            Value = null;
            return true;
        }

        throw new JsonException($"Invalid null value at line {_line}, column {_column}");
    }

    private bool ReadLiteral(string literal)
    {
        for (int i = 0; i < literal.Length; i++)
        {
            if (!EnsureBuffer() || _buffer[_bufferPosition] != literal[i])
                return false;
            ConsumeChar();
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDigit(char c) => c >= '0' && c <= '9';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsHexDigit(char c) =>
        (c >= '0' && c <= '9') ||
        (c >= 'A' && c <= 'F') ||
        (c >= 'a' && c <= 'f');

    public void Dispose()
    {
        if (!_disposed)
        {
            _buffer = null!;

            if (_ownsReader)
            {
                _reader?.Dispose();
            }

            _disposed = true;
        }
    }
}

/// <summary>
/// JSON �����쳣
/// </summary>
public class JsonException : Exception
{
    public JsonException(string message) : base(message) { }
    public JsonException(string message, Exception innerException) : base(message, innerException) { }
}