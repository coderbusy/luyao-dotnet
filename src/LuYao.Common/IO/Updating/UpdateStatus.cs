namespace LuYao.IO.Updating;

/// <summary>
/// 表示更新状态的枚举类型。
/// </summary>
public enum UpdateStatus
{
    /// <summary>
    /// 正在检查更新。
    /// </summary>
    Checking,

    /// <summary>
    /// 正在更新。
    /// </summary>
    Updating,

    /// <summary>
    /// 更新成功。
    /// </summary>
    Success,

    /// <summary>
    /// 更新失败。
    /// </summary>
    Fail
}
