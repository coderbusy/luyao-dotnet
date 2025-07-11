namespace LuYao.IO.Hosts;

/// <summary>
/// 表示主机文件中的一行类型。
/// </summary>
public enum HostLineType
{
    /// <summary>
    /// 空行。
    /// </summary>
    Blank,
    /// <summary>
    /// 注释行。
    /// </summary>
    Comment,
    /// <summary>
    /// 记录行。
    /// </summary>
    Record,
    /// <summary>
    /// 无效行。
    /// </summary>
    Invalid
}

/// <summary>
/// 定义主机文件行的接口。
/// </summary>
public interface IHostLine
{
    /// <summary>
    /// 获取行的类型。
    /// </summary>
    HostLineType LineType { get; }

    /// <summary>
    /// 将行转换为字符串表示形式。
    /// </summary>
    string ToString();
}

/// <summary>
/// 表示无效的主机文件行。
/// </summary>
public class InvalidLine(string raw) : IHostLine
{
    /// <summary>
    /// 获取或设置原始行内容。
    /// </summary>
    public string Raw { get; set; } = raw;

    /// <summary>
    /// 获取行的类型，始终为 <see cref="HostLineType.Invalid"/>。
    /// </summary>
    public HostLineType LineType => HostLineType.Invalid;

    /// <summary>
    /// 将行转换为字符串表示形式。
    /// </summary>
    public override string ToString() => Raw;
}

/// <summary>
/// 表示主机文件中的记录行。
/// </summary>
public class RecordLine : IHostLine
{
    /// <summary>
    /// 获取行的类型，始终为 <see cref="HostLineType.Record"/>。
    /// </summary>
    public HostLineType LineType => HostLineType.Record;

    /// <summary>
    /// 初始化记录行的新实例。
    /// </summary>
    /// <param name="ip">IP地址。</param>
    /// <param name="domain">域名。</param>
    public RecordLine(string ip, string domain)
    {
        IPAddress = ip;
        Domain = domain;
    }

    /// <summary>
    /// 获取或设置IP地址。
    /// </summary>
    public string IPAddress { get; set; }

    /// <summary>
    /// 获取或设置域名。
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// 获取或设置注释。
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// 将行转换为字符串表示形式。
    /// </summary>
    public override string ToString() => $"{IPAddress}\t{Domain}\t{Comment}".Trim();
}

/// <summary>
/// 表示主机文件中的注释行。
/// </summary>
public class CommentLine(string comment) : IHostLine
{
    /// <summary>
    /// 获取或设置注释内容。
    /// </summary>
    public string Comment { get; set; } = comment;

    /// <summary>
    /// 获取行的类型，始终为 <see cref="HostLineType.Comment"/>。
    /// </summary>
    public HostLineType LineType => HostLineType.Comment;

    /// <summary>
    /// 将行转换为字符串表示形式。
    /// </summary>
    public override string ToString() => Comment;
}

/// <summary>
/// 表示主机文件中的空行。
/// </summary>
public class BlankLine : IHostLine
{
    /// <summary>
    /// 获取空行的单例实例。
    /// </summary>
    public static BlankLine Instance { get; } = new();

    private BlankLine()
    {
    }

    /// <summary>
    /// 获取行的类型，始终为 <see cref="HostLineType.Blank"/>。
    /// </summary>
    public HostLineType LineType => HostLineType.Blank;

    /// <summary>
    /// 将行转换为字符串表示形式。
    /// </summary>
    public override string ToString() => string.Empty;
}