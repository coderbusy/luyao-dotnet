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

            if (cpuinfo.TryGetValue("Serial", out str) && 
                !String.IsNullOrEmpty(str) && 
                str.Trim('0') != "")
                UUID = str;
        }

        // 读取 machine-id
        var mid = "/etc/machine-id";
        if (!File.Exists(mid)) mid = "/var/lib/dbus/machine-id";
        if (TryRead(mid, out var value))
            Guid = value;

        // 读取 UUID
        var uuid = "";
        var file = "/sys/class/dmi/id/product_uuid";
        if (!File.Exists(file)) file = "/etc/uuid";
        if (!File.Exists(file)) file = "/proc/serial_num";
        if (TryRead(file, out value))
            uuid = value;
        if (!String.IsNullOrEmpty(uuid)) UUID = uuid;

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

        // 获取内存信息
        var meminfo = ReadInfo("/proc/meminfo");
        if (meminfo != null)
        {
            if (meminfo.TryGetValue("MemTotal", out str))
            {
                var match = System.Text.RegularExpressions.Regex.Match(str, @"(\d+)");
                if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var mem))
                    Memory = mem * 1024; // kB to Bytes
            }
        }
    }

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
#endif
    private void RefreshLinux()
    {
        // 刷新内存信息
        var meminfo = ReadInfo("/proc/meminfo");
        if (meminfo != null)
        {
            if (meminfo.TryGetValue("MemTotal", out var str))
            {
                var match = System.Text.RegularExpressions.Regex.Match(str, @"(\d+)");
                if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var mem))
                    Memory = mem * 1024;
            }

            if (meminfo.TryGetValue("MemAvailable", out str))
            {
                var match = System.Text.RegularExpressions.Regex.Match(str, @"(\d+)");
                if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var mem))
                    AvailableMemory = mem * 1024;
            }

            if (meminfo.TryGetValue("MemFree", out str))
            {
                var match = System.Text.RegularExpressions.Regex.Match(str, @"(\d+)");
                if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var mem))
                    FreeMemory = mem * 1024;
            }
        }

        // 获取CPU使用率
        try
        {
            var stat = File.ReadAllText("/proc/stat");
            var lines = stat.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("cpu "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 5)
                    {
                        var user = Int64.Parse(parts[1]);
                        var nice = Int64.Parse(parts[2]);
                        var system = Int64.Parse(parts[3]);
                        var idle = Int64.Parse(parts[4]);

                        var total = user + nice + system + idle;

                        if (_systemTime != null)
                        {
                            var idleDelta = idle - _systemTime.IdleTime;
                            var totalDelta = total - _systemTime.TotalTime;

                            if (totalDelta > 0)
                            {
                                CpuRate = 1.0 - ((double)idleDelta / totalDelta);
                                if (CpuRate < 0) CpuRate = 0;
                                if (CpuRate > 1) CpuRate = 1;
                            }
                        }

                        _systemTime = new SystemTime { IdleTime = idle, TotalTime = total };
                    }
                    break;
                }
            }
        }
        catch
        {
            // 忽略CPU使用率获取错误
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
