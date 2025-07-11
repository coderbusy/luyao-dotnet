using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace LuYao.IO.Hosts;

/// <summary>
/// 表示一个 Host 文件。
/// </summary>
public partial class HostFile
{
    private static readonly Regex RecordRegex = new Regex("^(?<ip>\\S+)\\s+(?<host>\\S+?)\\s*(?<com>#.+)?$");

    /// <summary>
    /// 从指定路径读取 Host 文件并解析为 HostFile 对象。
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>解析后的 HostFile 对象</returns>
    public static HostFile Read(string path)
    {
        if (File.Exists(path))
        {
            using var reader = new StreamReader(path, true);
            return Read(reader);
        }

        return new HostFile();
    }

    /// <summary>
    /// 从字符串集合中读取 Host 文件内容并解析为 HostFile 对象。
    /// </summary>
    /// <param name="lines">字符串集合，每行代表 Host 文件的一行内容</param>
    /// <returns>解析后的 HostFile 对象</returns>
    public static HostFile Read(IEnumerable<string> lines)
    {
        var ret = new HostFile();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                ret.Lines.Add(BlankLine.Instance);
            }
            else
            {
                var str = line.Trim();
                if (str.StartsWith("#"))
                {
                    ret.Lines.Add(new CommentLine(str));
                }
                else
                {
                    var m = RecordRegex.Match(str);
                    if (m.Success && IPAddress.TryParse(m.Groups["ip"].Value, out var _))
                    {
                        var record = new RecordLine(m.Groups["ip"].Value, m.Groups["host"].Value)
                        {
                            Comment = m.Groups["com"].Value
                        };
                        ret.Lines.Add(record);
                    }
                    else
                    {
                        ret.Lines.Add(new InvalidLine(line));
                    }
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// 从 TextReader 对象中读取 Host 文件内容并解析为 HostFile 对象。
    /// </summary>
    /// <param name="reader">TextReader 对象</param>
    /// <returns>解析后的 HostFile 对象</returns>
    public static HostFile Read(TextReader reader)
    {
        var lines = new List<string>();
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null) break;
            lines.Add(line!);
        }

        return Read(lines);
    }

    /// <summary>
    /// 从字符串内容解析 Host 文件并生成 HostFile 对象。
    /// </summary>
    /// <param name="content">Host 文件的字符串内容</param>
    /// <returns>解析后的 HostFile 对象</returns>
    public static HostFile Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return new HostFile();
        using (var sr = new StringReader(content)) return Read(sr);
    }

    /// <summary>
    /// 获取 Host 文件的所有行。
    /// </summary>
    public List<IHostLine> Lines { get; } = new();

    /// <summary>
    /// 将 HostFile 对象转换为字符串形式，包含所有行内容。
    /// </summary>
    /// <returns>Host 文件的字符串表示</returns>
    public override string ToString()
    {
        return string.Join(Environment.NewLine, this.Lines.Select(f => f.ToString()));
    }
}