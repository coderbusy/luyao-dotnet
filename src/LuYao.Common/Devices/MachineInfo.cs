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

    /// <summary>硬件唯一标识。取主板编码，部分品牌存在重复</summary>
    [DisplayName("硬件唯一标识")]
    public String? UUID { get; set; }

    /// <summary>软件唯一标识。系统标识，操作系统重装后更新，Linux系统的machine_id，Android的android_id，Ghost系统存在重复</summary>
    [DisplayName("软件唯一标识")]
    public String? Guid { get; set; }

    /// <summary>计算机序列号。适用于品牌机，跟笔记本标签显示一致</summary>
    [DisplayName("计算机序列号")]
    public String? Serial { get; set; }

    /// <summary>主板。序列号或家族信息</summary>
    [DisplayName("主板")]
    public String? Board { get; set; }

    /// <summary>磁盘序列号</summary>
    [DisplayName("磁盘序列号")]
    public String? DiskID { get; set; }

    /// <summary>内存总量。单位Byte</summary>
    [DisplayName("内存总量")]
    public UInt64 Memory { get; set; }

    /// <summary>可用内存。单位Byte</summary>
    [DisplayName("可用内存")]
    public UInt64 AvailableMemory { get; set; }

    /// <summary>空闲内存。在Linux上空闲内存不一定可用，单位Byte</summary>
    [DisplayName("空闲内存")]
    public UInt64 FreeMemory { get; set; }

    /// <summary>CPU占用率</summary>
    [DisplayName("CPU占用率")]
    public Double CpuRate { get; set; }

    /// <summary>网络上行速度。字节每秒，初始化后首次读取为0</summary>
    [DisplayName("网络上行速度")]
    public UInt64 UplinkSpeed { get; set; }

    /// <summary>网络下行速度。字节每秒，初始化后首次读取为0</summary>
    [DisplayName("网络下行速度")]
    public UInt64 DownlinkSpeed { get; set; }

    /// <summary>温度。单位度</summary>
    [DisplayName("温度")]
    public Double Temperature { get; set; }

    /// <summary>电池剩余。小于1的小数，常用百分比表示</summary>
    [DisplayName("电池剩余")]
    public Double Battery { get; set; }

    private readonly Dictionary<String, Object?> _items = new Dictionary<String, Object?>();

    /// <summary>获取 或 设置 扩展属性数据</summary>
    /// <param name="key">属性键名</param>
    /// <returns>属性值</returns>
    public Object? this[String key] 
    { 
        get => _items.TryGetValue(key, out var obj) ? obj : null; 
        set => _items[key] = value; 
    }
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
        UUID = Clean(UUID);
        Guid = Clean(Guid);
        Serial = Clean(Serial);
        Board = Clean(Board);
        DiskID = Clean(DiskID);

        // 无法读取系统标识时，随机生成一个guid
        if (String.IsNullOrEmpty(Guid)) 
            Guid = "0-" + System.Guid.NewGuid().ToString();
        if (String.IsNullOrEmpty(UUID)) 
            UUID = "0-" + System.Guid.NewGuid().ToString();

        try
        {
            Refresh();
        }
        catch
        {
            // 忽略刷新错误
        }
    }

    /// <summary>
    /// 刷新动态性能信息（CPU、内存、网络等）
    /// </summary>
    public void Refresh()
    {
#if NETFRAMEWORK
        // .NET Framework only runs on Windows
        RefreshWindows();
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            RefreshWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            RefreshLinux();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            RefreshMacOS();
        }
#endif

        RefreshSpeed();
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
    private static String? Clean(String? value)
    {
        if (String.IsNullOrEmpty(value)) return value;
        
        var sb = new StringBuilder();
        foreach (var c in value)
        {
            if (c >= 32 && c != 127) // 过滤控制字符
                sb.Append(c);
        }
        return sb.ToString().Trim();
    }
    #endregion

    #region 平台相关私有类
    private class SystemTime
    {
        public Int64 IdleTime;
        public Int64 TotalTime;
    }

    private SystemTime? _systemTime;
    #endregion
}
