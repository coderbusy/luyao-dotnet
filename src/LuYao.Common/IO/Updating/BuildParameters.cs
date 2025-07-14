using LuYao.Logging;
using LuYao.Logging.Loggers;

namespace LuYao.IO.Updating;

/// <summary>
/// 构建更新参数的类，用于定义更新操作的相关配置。
/// </summary>
public class BuildParameters
{
    /// <summary>
    /// 更新源路径，目录的路径。
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// 更新目标分发路径，目录的路径。
    /// </summary>
    public string Distribution { get; set; }

    /// <summary>
    /// 更新版本号，用于标识当前更新的版本。
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 更新描述信息，用于记录更新的详细说明。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 基础 URL，用于构建更新的网络资源路径。
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// 日志记录器，用于记录更新过程中的日志信息。
    /// 默认值为 NullLogger 实例。
    /// </summary>
    public ILogger Logger { get; set; } = NullLogger.Instance;

    /// <summary>
    /// 是否清理目标路径，设置为 true 表示在更新前清理目标路径。
    /// </summary>
    public bool Clear { get; set; }

    /// <summary>
    /// 完整文件的文件名
    /// </summary>
    public string WholeFile { get; set; }
}
