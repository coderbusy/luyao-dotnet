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
        var str = GetLinuxName();
        if (!String.IsNullOrEmpty(str)) OSName = str;

        // 读取 /proc/cpuinfo
        var cpuinfo = ReadInfo("/proc/cpuinfo");
        if (cpuinfo != null)
        {
            if (cpuinfo.TryGetValue("Hardware", out str) ||
                cpuinfo.TryGetValue("cpu model", out str) ||
                cpuinfo.TryGetValue("model name", out str))
            {
                Processor = str;
                if (Processor != null && Processor.StartsWith("vendor "))
                    Processor = Processor.Substring(7);
            }

            if (cpuinfo.TryGetValue("Model", out str))
                Product = str;

            if (cpuinfo.TryGetValue("vendor_id", out str))
                Vendor = str;
        }

        // 从release文件读取产品
        var prd = GetProductByRelease();
        if (!String.IsNullOrEmpty(prd)) Product = prd;

        if (String.IsNullOrEmpty(prd) && TryRead("/sys/class/dmi/id/product_name", out var product_name))
        {
            Product = product_name;
        }

        if (TryRead("/sys/class/dmi/id/sys_vendor", out var sys_vendor))
        {
            Vendor = sys_vendor;
        }

        if (TryRead("/sys/class/dmi/id/board_serial", out var board_serial))
        {
            Board = board_serial;
        }

        if (TryRead("/sys/class/dmi/id/product_serial", out var product_serial))
        {
            Serial = product_serial;
        }

        // 获取磁盘序列号 (简化版，仅获取第一个硬盘)
        try
        {
            var diskDir = "/sys/block/";
            if (Directory.Exists(diskDir))
            {
                foreach (var disk in Directory.GetDirectories(diskDir))
                {
                    var diskName = Path.GetFileName(disk);
                    if (diskName.StartsWith("sd") || diskName.StartsWith("nvme") || diskName.StartsWith("hd"))
                    {
                        var serialFile = Path.Combine(disk, "device", "serial");
                        if (TryRead(serialFile, out var diskSerial))
                        {
                            DiskID = diskSerial;
                            break;
                        }
                    }
                }
            }
        }
        catch
        {
            // 忽略磁盘序列号读取错误
        }
    }

    #region Linux辅助方法
    /// <summary>获取Linux发行版名称</summary>
    /// <returns>Linux发行版名称</returns>
    private static String? GetLinuxName()
    {
        var fr = "/etc/redhat-release";
        if (TryRead(fr, out var value)) return value;

        var dr = "/etc/debian-release";
        if (TryRead(dr, out value)) return value;

        var sr = "/etc/os-release";
        if (TryRead(sr, out value))
        {
            var dic = SplitAsDictionary(value, "=", "\n");
            if (dic.TryGetValue("PRETTY_NAME", out var pretty) && !String.IsNullOrEmpty(pretty)) 
                return pretty.Trim('"');
            if (dic.TryGetValue("NAME", out var name) && !String.IsNullOrEmpty(name)) 
                return name.Trim('"');
        }

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
            if (process != null)
            {
                var uname = process.StandardOutput.ReadToEnd()?.Trim();
                process.WaitForExit();
                
                if (!String.IsNullOrEmpty(uname))
                {
                    // 支持Android系统名
                    var ss = uname.Split('-');
                    foreach (var item in ss)
                    {
                        if (!String.IsNullOrEmpty(item) && 
                            item.StartsWith("Android", StringComparison.OrdinalIgnoreCase))
                            return item;
                    }
                    return uname;
                }
            }
        }
        catch
        {
            // 忽略错误
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
