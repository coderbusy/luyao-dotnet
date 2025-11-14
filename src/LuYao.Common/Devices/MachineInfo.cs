using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Devices;

/// <summary>
/// 机器信息类，用于获取和存储机器的硬件和系统信息
/// </summary>
/// <remarks>
/// 该类提供了跨平台的机器信息获取功能，包括操作系统信息、硬件标识、性能指标等。
/// 支持 Windows、Linux 和 macOS 操作系统。
/// 刷新信息成本较高，建议采用单例模式或缓存机制。
/// </remarks>
public partial class MachineInfo
{
    #region 属性
    /// <summary>系统名称</summary>
    [DisplayName("系统名称")]
    public String? OSName { get; set; }

    /// <summary>系统版本</summary>
    [DisplayName("系统版本")]
    public String? OSVersion { get; set; }

    /// <summary>产品名称</summary>
    [DisplayName("产品名称")]
    public String? Product { get; set; }

    /// <summary>制造商</summary>
    [DisplayName("制造商")]
    public String? Vendor { get; set; }

    /// <summary>处理器型号</summary>
    [DisplayName("处理器型号")]
    public String? Processor { get; set; }

    /// <summary>计算机序列号。适用于品牌机，跟笔记本标签显示一致</summary>
    [DisplayName("计算机序列号")]
    public String? Serial { get; set; }

    /// <summary>主板。序列号或家族信息</summary>
    [DisplayName("主板")]
    public String? Board { get; set; }

    /// <summary>磁盘序列号</summary>
    [DisplayName("磁盘序列号")]
    public String? DiskID { get; set; }
    #endregion

    #region 静态方法
    /// <summary>
    /// 获取机器信息的静态实例
    /// </summary>
    /// <returns>初始化后的机器信息实例</returns>
    public static MachineInfo Get()
    {
        var info = new MachineInfo();
        info.Init();
        return info;
    }
    #endregion

    #region 初始化和刷新
    /// <summary>
    /// 初始化机器信息，加载静态硬件信息
    /// </summary>
    public void Init()
    {
        Reset();
        
#if NETFRAMEWORK
        // .NET Framework only runs on Windows
        LoadWindowsInfo();
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoadWindowsInfo();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LoadLinuxInfo();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            LoadMacInfo();
        }
#endif

        // 清理数据
        OSName = Clean(OSName);
        OSVersion = Clean(OSVersion);
        Product = Clean(Product);
        Vendor = Clean(Vendor);
        Processor = Clean(Processor);
        Serial = Clean(Serial);
        Board = Clean(Board);
        DiskID = Clean(DiskID);
    }

    /// <summary>
    /// 重新加载机器信息
    /// </summary>
    public void Reload()
    {
        Init();
    }

    private void Reset()
    {
#if NETFRAMEWORK
        OSName = Environment.OSVersion.Platform.ToString();
        OSVersion = Environment.OSVersion.VersionString;
#else
        OSName = RuntimeInformation.OSDescription;
        OSVersion = Environment.OSVersion.VersionString;
#endif
    }

    /// <summary>裁剪不可见字符并去除两端空白</summary>
    /// <param name="value">待清理的字符串</param>
    /// <returns>清理后的字符串</returns>
    private static String? Clean(String? value)
    {
        if (String.IsNullOrEmpty(value)) 
            return value;
        
        // 快速路径：检查是否包含控制字符
        if (!ContainsControlCharacters(value))
            return value.Trim();
        
        // 慢速路径：清理控制字符后返回
        return RemoveControlCharacters(value).Trim();
    }

    /// <summary>检查字符串是否包含控制字符</summary>
    private static Boolean ContainsControlCharacters(String value)
    {
        const Int32 MinPrintableChar = 32;
        const Int32 DeleteChar = 127;
        
        foreach (var c in value)
        {
            if (c < MinPrintableChar || c == DeleteChar)
                return true;
        }
        return false;
    }

    /// <summary>移除字符串中的控制字符</summary>
    private static String RemoveControlCharacters(String value)
    {
        const Int32 MinPrintableChar = 32;
        const Int32 DeleteChar = 127;
        
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c >= MinPrintableChar && c != DeleteChar)
                sb.Append(c);
        }
        return sb.ToString();
    }
    #endregion

}
