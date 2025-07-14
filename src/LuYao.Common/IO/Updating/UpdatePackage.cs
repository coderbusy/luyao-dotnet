using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LuYao.IO.Updating;

/// <summary>
/// 更新包
/// </summary>
[Serializable]
public class UpdatePackage
{
    /// <summary>
    /// 初始化一个新的更新包实例。
    /// </summary>
    public UpdatePackage()
    {
        UpdateFilePackages = new UpdateFilePackageCollection();
        BuildTime = DateTimeOffset.Now;
    }

    /// <summary>
    /// 更新包的描述信息。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 更新包的构建时间。
    /// </summary>
    public DateTimeOffset BuildTime { get; set; }

    /// <summary>
    /// 更新包的版本号。
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// 更新包的基础 URL，用于下载更新文件。
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// 更新文件包集合，包含所有需要更新的文件信息。
    /// </summary>
    public UpdateFilePackageCollection UpdateFilePackages { get; }

    /// <summary>
    /// 检查指定程序集和目录是否需要更新。
    /// </summary>
    /// <param name="assembly">要检查的程序集。</param>
    /// <param name="dir">要检查的目录路径。</param>
    /// <returns>如果需要更新，返回 true；否则返回 false。</returns>
    public async Task<bool> HasUpdate(Assembly assembly, string dir)
    {
        var name = assembly.GetName();
        if (name.Version != null && name.Version.ToString() == Version) return false;
        foreach (var pkg in this.UpdateFilePackages)
        {
            var fn = Path.Combine(dir, pkg.FilePath);
            if (!File.Exists(fn)) return true;
            if (await UpdatePackageHelper.Hash(fn) != pkg.FileHash)
                return true;
        }
        return false;
    }
}
