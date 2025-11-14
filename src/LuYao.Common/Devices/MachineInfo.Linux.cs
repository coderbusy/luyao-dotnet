using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

namespace LuYao.Devices;

/// <summary>
/// MachineInfo 的 Linux 平台实现部分
/// </summary>
public partial class MachineInfo
{
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
#endif
    private void LoadLinuxInfo()
    {
        // 优先从 DMI 读取信息（更快且更可靠）
        var hasDMI = TryReadDMIInfo();
        
        // 只有在 DMI 信息不完整时才读取其他来源
        if (String.IsNullOrEmpty(Processor))
        {
            var cpuinfo = ReadInfo("/proc/cpuinfo");
            if (cpuinfo != null)
            {
                if (cpuinfo.TryGetValue("Hardware", out var str) ||
                    cpuinfo.TryGetValue("cpu model", out str) ||
                    cpuinfo.TryGetValue("model name", out str))
                {
                    Processor = str;
                    if (Processor != null && Processor.StartsWith("vendor "))
                        Processor = Processor.Substring(7);
                }

                if (String.IsNullOrEmpty(Product) && cpuinfo.TryGetValue("Model", out str))
                    Product = str;

                if (String.IsNullOrEmpty(Vendor) && cpuinfo.TryGetValue("vendor_id", out str))
                    Vendor = str;
            }
        }

        // 获取 OS 名称
        if (String.IsNullOrEmpty(OSName))
        {
            var str = GetLinuxName();
            if (!String.IsNullOrEmpty(str)) OSName = str;
        }

        // 从 release 文件读取产品（仅在需要时）
        if (String.IsNullOrEmpty(Product) && !hasDMI)
        {
            var prd = GetProductByRelease();
            if (!String.IsNullOrEmpty(prd)) Product = prd;
        }

        // 获取磁盘序列号（仅在需要时）
        if (String.IsNullOrEmpty(DiskID))
        {
            TryReadDiskSerial();
        }
    }
    
    /// <summary>尝试读取 DMI 信息（一次性读取多个字段以提高性能）</summary>
    /// <returns>如果成功读取任何DMI信息则返回true</returns>
    private Boolean TryReadDMIInfo()
    {
        const String DmiBasePath = "/sys/class/dmi/id/";
        
        if (!Directory.Exists(DmiBasePath))
            return false;
        
        var hasData = false;
        
        // 读取产品名称
        if (TryReadDmiField(DmiBasePath, "product_name", out var productName))
        {
            Product = productName;
            hasData = true;
        }
        
        // 读取系统供应商
        if (TryReadDmiField(DmiBasePath, "sys_vendor", out var sysVendor))
        {
            Vendor = sysVendor;
            hasData = true;
        }
        
        // 读取主板序列号
        if (TryReadDmiField(DmiBasePath, "board_serial", out var boardSerial))
        {
            Board = boardSerial;
            hasData = true;
        }
        
        // 读取产品序列号
        if (TryReadDmiField(DmiBasePath, "product_serial", out var productSerial))
        {
            Serial = productSerial;
            hasData = true;
        }
        
        return hasData;
    }

    /// <summary>尝试读取DMI字段</summary>
    private static Boolean TryReadDmiField(String basePath, String fileName, out String? value)
    {
        return TryRead(Path.Combine(basePath, fileName), out value);
    }
    
    /// <summary>尝试读取磁盘序列号</summary>
    private void TryReadDiskSerial()
    {
        const String BlockDevicePath = "/sys/block/";
        
        try
        {
            if (!Directory.Exists(BlockDevicePath))
                return;
            
            // 只检查物理磁盘（跳过循环设备等）
            foreach (var diskPath in Directory.GetDirectories(BlockDevicePath))
            {
                var diskName = Path.GetFileName(diskPath);
                
                if (!IsPhysicalDisk(diskName))
                    continue;

                var serialFile = Path.Combine(diskPath, "device", "serial");
                if (TryRead(serialFile, out var diskSerial))
                {
                    DiskID = diskSerial;
                    return; // 找到第一个物理磁盘序列号即返回
                }
            }
        }
        catch
        {
            // 忽略磁盘序列号读取错误
        }
    }

    /// <summary>判断是否为物理磁盘</summary>
    private static Boolean IsPhysicalDisk(String diskName)
    {
        return diskName.StartsWith("sd") ||    // SCSI/SATA磁盘
               diskName.StartsWith("nvme") ||  // NVMe磁盘
               diskName.StartsWith("hd");      // IDE磁盘
    }

    #region Linux辅助方法
    /// <summary>获取Linux发行版名称</summary>
    /// <returns>Linux发行版名称</returns>
    private static String? GetLinuxName()
    {
        // 按优先级尝试各种方式获取OS名称
        return TryGetFromReleaseFiles() ?? TryGetFromUname();
    }

    /// <summary>从发行版文件获取OS名称</summary>
    private static String? TryGetFromReleaseFiles()
    {
        // 尝试 RedHat 系列
        if (TryRead("/etc/redhat-release", out var value))
            return value;

        // 尝试 Debian 系列
        if (TryRead("/etc/debian-release", out value))
            return value;

        // 尝试通用 os-release 文件
        if (TryRead("/etc/os-release", out value))
            return ParseOsRelease(value);

        return null;
    }

    /// <summary>解析 os-release 文件内容</summary>
    private static String? ParseOsRelease(String content)
    {
        var dic = SplitAsDictionary(content, "=", "\n");
        
        // 优先使用 PRETTY_NAME
        if (dic.TryGetValue("PRETTY_NAME", out var pretty) && !String.IsNullOrEmpty(pretty))
            return pretty.Trim('"');
        
        // 退而求其次使用 NAME
        if (dic.TryGetValue("NAME", out var name) && !String.IsNullOrEmpty(name))
            return name.Trim('"');

        return null;
    }

    /// <summary>从 uname 命令获取OS名称</summary>
    private static String? TryGetFromUname()
    {
        const Int32 UnameTimeoutMs = 2000;
        
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "uname",
                Arguments = "-sr",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
                return null;

            var uname = process.StandardOutput.ReadToEnd()?.Trim();
            
            if (!process.WaitForExit(UnameTimeoutMs))
            {
                try { process.Kill(); } catch { }
                return null;
            }
            
            if (String.IsNullOrEmpty(uname))
                return null;

            // 特殊处理：提取 Android 系统名
            return ExtractAndroidName(uname) ?? uname;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>从 uname 输出中提取 Android 系统名</summary>
    private static String? ExtractAndroidName(String uname)
    {
        var parts = uname.Split('-');
        foreach (var part in parts)
        {
            if (!String.IsNullOrEmpty(part) && 
                part.StartsWith("Android", StringComparison.OrdinalIgnoreCase))
                return part;
        }
        return null;
    }

    private static String? GetProductByRelease()
    {
        try
        {
            if (!Directory.Exists("/etc/")) return null;

            var files = Directory.GetFiles("/etc/", "*-release");
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (!fileName.Equals("redhat-release", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.Equals("debian-release", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.Equals("os-release", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.Equals("system-release", StringComparison.OrdinalIgnoreCase))
                {
                    var content = File.ReadAllText(file);
                    var dic = SplitAsDictionary(content, "=", "\n");
                    if (dic.TryGetValue("BOARD", out var str)) return str;
                    if (dic.TryGetValue("BOARD_NAME", out str)) return str;
                }
            }
        }
        catch
        {
            // 忽略错误
        }

        return null;
    }

    private static Boolean TryRead(String fileName, out String? value)
    {
        value = null;

        if (!File.Exists(fileName)) return false;

        try
        {
            value = File.ReadAllText(fileName)?.Trim();
            if (String.IsNullOrEmpty(value)) return false;
        }
        catch { return false; }

        return true;
    }

    /// <summary>读取文件信息，分割为字典</summary>
    /// <param name="file">文件路径</param>
    /// <param name="separate">分隔符</param>
    /// <returns>解析后的字典</returns>
    private static Dictionary<String, String?>? ReadInfo(String file, Char separate = ':')
    {
        if (String.IsNullOrEmpty(file) || !File.Exists(file)) return null;

        var dic = new Dictionary<String, String?>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    var p = line.IndexOf(separate);
                    if (p > 0)
                    {
                        var key = line.Substring(0, p).Trim();
                        var value = line.Substring(p + 1).Trim();
                        dic[key] = Clean(value);
                    }
                }
            }
        }
        catch
        {
            return null;
        }

        return dic;
    }

    private static Dictionary<String, String> SplitAsDictionary(String content, String keyValueSeparator, String lineSeparator)
    {
        var dic = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        
        if (String.IsNullOrEmpty(content)) return dic;

        var lines = content.Split(new[] { lineSeparator }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split(new[] { keyValueSeparator }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');
                if (!String.IsNullOrEmpty(key))
                    dic[key] = value;
            }
        }

        return dic;
    }
    #endregion
}
