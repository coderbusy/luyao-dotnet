using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LuYao.Data;

namespace LuYao.IO.Ini;

/// <summary>
/// INI 文件读写器，使用 Record 类型承载数据。
/// </summary>
public class IniFile
{
    private static readonly Regex SectionRegex = new Regex(@"^\s*\[(?<section>[^\]]+)\]\s*(?<comment>[;#].*)?$");
    private static readonly Regex KeyValueRegex = new Regex(@"^\s*(?<key>[^=]+?)\s*=\s*(?<value>.*?)\s*(?<comment>[;#].*)?$");
    private static readonly Regex CommentRegex = new Regex(@"^\s*(?<comment>[;#].*)$");

    private readonly RecordColumn _Section;
    private readonly RecordColumn _Key;
    private readonly RecordColumn _Value;
    private readonly RecordColumn _Comment;

    /// <summary>
    /// 获取包含 INI 数据的 Record 对象。
    /// 包含四列：Section、Key、Value、Comment
    /// </summary>
    public Record Data { get; }
    /// <summary>
    /// 初始化 IniFile 的新实例。
    /// </summary>
    public IniFile()
    {
        Data = new Record("IniData", 20);
        this._Section = Data.Columns.AddString("Section");
        this._Key = Data.Columns.AddString("Key");
        this._Value = Data.Columns.AddString("Value");
        this._Comment = Data.Columns.AddString("Comment");
    }

    /// <summary>
    /// 从指定路径读取 INI 文件。
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>IniFile 实例</returns>
    public static IniFile Read(string path)
    {
        if (!File.Exists(path))
            return new IniFile();

        var content = File.ReadAllText(path);
        return Parse(content);
    }

    /// <summary>
    /// 从字符串内容解析 INI 文件。
    /// </summary>
    /// <param name="content">INI 文件内容</param>
    /// <returns>IniFile 实例</returns>
    public static IniFile Parse(string content)
    {
        var iniFile = new IniFile();
        if (string.IsNullOrWhiteSpace(content))
            return iniFile;

        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var currentSection = string.Empty;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // 检查注释行
            var commentMatch = CommentRegex.Match(line);
            if (commentMatch.Success)
            {
                iniFile.AddRow(currentSection, string.Empty, string.Empty, commentMatch.Groups["comment"].Value);
                continue;
            }

            // 检查节行
            var sectionMatch = SectionRegex.Match(line);
            if (sectionMatch.Success)
            {
                currentSection = sectionMatch.Groups["section"].Value.Trim();
                var comment = sectionMatch.Groups["comment"].Success ? sectionMatch.Groups["comment"].Value : string.Empty;
                iniFile.AddRow(currentSection, string.Empty, string.Empty, comment);
                continue;
            }

            // 检查键值对行
            var keyValueMatch = KeyValueRegex.Match(line);
            if (keyValueMatch.Success)
            {
                var key = keyValueMatch.Groups["key"].Value.Trim();
                var value = keyValueMatch.Groups["value"].Value.Trim();
                var comment = keyValueMatch.Groups["comment"].Success ? keyValueMatch.Groups["comment"].Value : string.Empty;
                iniFile.AddRow(currentSection, key, value, comment);
                continue;
            }
        }

        return iniFile;
    }

    /// <summary>
    /// 向 Record 中添加一行数据。
    /// </summary>
    private void AddRow(string section, string key, string value, string comment)
    {
        var row = Data.AddRow();
        row.Set(section, _Section);
        row.Set(key, _Key);
        row.Set(value, _Value);
        row.Set(comment, _Comment);
    }

    /// <summary>
    /// 获取指定节中指定键的值。
    /// </summary>
    /// <param name="sectionName">节名称</param>
    /// <param name="keyName">键名</param>
    /// <returns>键值，未找到返回 null</returns>
    public string? GetValue(string sectionName, string keyName)
    {
        foreach (var row in this.Data)
        {
            string section = row.ToString(_Section);
            string key = row.ToString(_Key);
            if (section == sectionName && key == keyName)
            {
                return row.ToString(_Value);
            }
        }
        return null;
    }

    /// <summary>
    /// 设置指定节中指定键的值。
    /// </summary>
    /// <param name="sectionName">节名称</param>
    /// <param name="keyName">键名</param>
    /// <param name="value">键值</param>
    public void SetValue(string sectionName, string keyName, string value)
    {
        // 查找现有键值对
        foreach (var row in this.Data)
        {
            string section = row.ToString(_Section);
            string key = row.ToString(_Key);
            if (section == sectionName && key == keyName)
            {
                row.Set(value, _Value);
                return;
            }
        }

        // 如果没找到，添加新键值对
        AddRow(sectionName, keyName, value, string.Empty);
    }

    /// <summary>
    /// 获取指定节中的所有键值对。
    /// </summary>
    /// <param name="sectionName">节名称</param>
    /// <returns>键值对字典</returns>
    public Dictionary<string, string> GetSection(string sectionName)
    {
        var result = new Dictionary<string, string>();

        for (int i = 0; i < Data.Count; i++)
        {
            var section = Data.GetValue("Section", i)?.ToString() ?? string.Empty;
            var key = Data.GetValue("Key", i)?.ToString() ?? string.Empty;
            var value = Data.GetValue("Value", i)?.ToString() ?? string.Empty;

            if (string.Equals(section, sectionName, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(key))
            {
                result[key] = value;
            }
        }

        return result;
    }

    /// <summary>
    /// 获取所有节名称。
    /// </summary>
    /// <returns>节名称集合</returns>
    public IEnumerable<string> GetSectionNames()
    {
        var sections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < Data.Count; i++)
        {
            var section = Data.GetValue("Section", i)?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(section))
            {
                sections.Add(section);
            }
        }

        return sections.OrderBy(s => s);
    }

    /// <summary>
    /// 将 INI 数据转换为字符串格式。
    /// </summary>
    /// <returns>INI 格式字符串</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        var currentSection = string.Empty;

        for (int i = 0; i < Data.Count; i++)
        {
            var section = Data.GetValue("Section", i)?.ToString() ?? string.Empty;
            var key = Data.GetValue("Key", i)?.ToString() ?? string.Empty;
            var value = Data.GetValue("Value", i)?.ToString() ?? string.Empty;
            var comment = Data.GetValue("Comment", i)?.ToString() ?? string.Empty;

            // 处理节变化
            if (!string.Equals(currentSection, section, StringComparison.OrdinalIgnoreCase))
            {
                if (sb.Length > 0)
                    sb.AppendLine();

                if (!string.IsNullOrEmpty(section))
                {
                    sb.Append($"[{section}]");
                    if (!string.IsNullOrEmpty(comment))
                        sb.Append($" {comment}");
                    sb.AppendLine();
                }
                currentSection = section;
            }

            // 处理键值对
            if (!string.IsNullOrEmpty(key))
            {
                sb.Append($"{key}={value}");
                if (!string.IsNullOrEmpty(comment))
                    sb.Append($" {comment}");
                sb.AppendLine();
            }
            // 处理纯注释行
            else if (!string.IsNullOrEmpty(comment))
            {
                sb.AppendLine(comment);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 将 INI 数据保存到文件。
    /// </summary>
    /// <param name="path">文件路径</param>
    public void Save(string path)
    {
        File.WriteAllText(path, ToString());
    }

    /// <summary>
    /// 将 INI 数据写入到 TextWriter。
    /// </summary>
    /// <param name="writer">TextWriter 对象</param>
    public void Save(TextWriter writer)
    {
        writer.Write(ToString());
    }
}
