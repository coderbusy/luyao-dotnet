using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.IO.Updating;

/// <summary>  
/// 表示一个更新文件包。  
/// </summary>  
[Serializable]
public class UpdateFilePackage
{
    /// <summary>
    ///     升级包的校验值
    /// </summary>
    public string PackageHash { get; set; }

    /// <summary>
    ///     升级包大小
    /// </summary>
    public long PackageSize { get; set; }

    /// <summary>
    ///     升级包路径
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     文件路径
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    ///     文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     文件校验值
    /// </summary>
    public string FileHash { get; set; }

    /// <summary>
    ///     文件版本
    /// </summary>
    public string FileVersion { get; set; }
}
