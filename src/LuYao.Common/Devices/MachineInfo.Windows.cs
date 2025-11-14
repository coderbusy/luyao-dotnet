using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

#if NETFRAMEWORK || NET5_0_OR_GREATER
using Microsoft.Win32;
#endif

namespace LuYao.Devices;

/// <summary>
/// MachineInfo 的 Windows 平台实现部分
/// </summary>
public partial class MachineInfo
{
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    private void LoadWindowsInfo()
    {
        var str = "";

        // 从注册表读取硬件信息
#if NETFRAMEWORK || NET6_0_OR_GREATER
        try
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
            reg ??= Registry.LocalMachine.OpenSubKey(@"SYSTEM\HardwareConfig\Current");
            if (reg != null)
            {
                Product = (reg.GetValue("SystemProductName")?.ToString() ?? "").Replace("System Product Name", "");
                if (String.IsNullOrEmpty(Product)) Product = reg.GetValue("BaseBoardProduct")?.ToString() ?? "";

                Vendor = reg.GetValue("SystemManufacturer")?.ToString() ?? "";
                if (String.IsNullOrEmpty(Vendor)) Vendor = reg.GetValue("BaseBoardManufacturer")?.ToString() ?? "";
            }

            reg = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            if (reg != null) Processor = reg.GetValue("ProcessorNameString")?.ToString() ?? "";
        }
        catch
        {
            // 忽略注册表访问错误
        }
#endif

        // 通过 wmic 获取更多信息
        if (String.IsNullOrEmpty(Vendor) || String.IsNullOrEmpty(Product))
        {
            var csproduct = ReadWmic("csproduct", "Name", "Vendor");
            if (csproduct != null)
            {
                if (csproduct.TryGetValue("Name", out str) && !String.IsNullOrEmpty(str) && String.IsNullOrEmpty(Product)) 
                    Product = str;
                if (csproduct.TryGetValue("Vendor", out str) && !String.IsNullOrEmpty(str)) 
                    Vendor = str;
            }
        }

        // 获取操作系统名称和版本
        try
        {
#if NETFRAMEWORK || NET6_0_OR_GREATER
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (reg != null)
            {
                OSName = reg.GetValue("ProductName")?.ToString() ?? "";
                var releaseId = reg.GetValue("ReleaseId")?.ToString() ?? "";
                if (!String.IsNullOrEmpty(releaseId))
                    OSVersion = releaseId;
            }
#endif
        }
        catch
        {
            // 使用默认值
        }

        // 通过 wmic 获取更多信息
        var os = ReadWmic("os", "Caption", "Version");
        if (os != null && os.Count > 0)
        {
            if (os.TryGetValue("Caption", out str)) 
                OSName = str.Replace("Microsoft", "").Trim();
            if (os.TryGetValue("Version", out str)) 
                OSVersion = str;
        }

        // 获取磁盘信息
        var disk = ReadWmic("diskdrive where mediatype=\"Fixed hard disk media\"", "serialnumber");
        if (disk != null && disk.TryGetValue("serialnumber", out str))
            DiskID = str?.Trim();

        // 获取BIOS序列号
        var bios = ReadWmic("bios", "serialnumber");
        if (bios != null && bios.TryGetValue("serialnumber", out str) && 
            !str.Equals("System Serial Number", StringComparison.OrdinalIgnoreCase))
            Serial = str?.Trim();

        // 获取主板序列号
        var board = ReadWmic("baseboard", "serialnumber");
        if (board != null && board.TryGetValue("serialnumber", out str))
            Board = str?.Trim();

        if (String.IsNullOrEmpty(OSName))
        {
#if NETFRAMEWORK
            OSName = Environment.OSVersion.Platform.ToString().Replace("Microsoft", "").Trim();
#else
            OSName = RuntimeInformation.OSDescription.Replace("Microsoft", "").Trim();
#endif
        }
        if (String.IsNullOrEmpty(OSVersion))
            OSVersion = Environment.OSVersion.Version.ToString();
    }

    #region Windows辅助方法
    /// <summary>通过WMIC命令读取信息</summary>
    /// <param name="type">WMI类型</param>
    /// <param name="keys">查询字段</param>
    /// <returns>解析后的字典</returns>
    private static Dictionary<String, String> ReadWmic(String type, params String[] keys)
    {
        var dic = new Dictionary<String, List<String>>(StringComparer.OrdinalIgnoreCase);
        var dic2 = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var args = $"{type} get {String.Join(",", keys)} /format:list";
            var psi = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!String.IsNullOrEmpty(output))
                {
                    var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (var line in lines)
                    {
                        var parts = line.Split('=');
                        if (parts.Length >= 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();
                            
                            // 清理不可见字符
                            value = Clean(value) ?? "";

                            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
                            {
                                if (!dic.TryGetValue(key, out var list))
                                    dic[key] = list = new List<String>();

                                list.Add(value);
                            }
                        }
                    }
                }
            }

            // 排序，避免多个磁盘序列号时，顺序变动
            foreach (var item in dic)
            {
                dic2[item.Key] = String.Join(",", item.Value.OrderBy(e => e));
            }
        }
        catch
        {
            // 忽略错误
        }

        return dic2;
    }
    #endregion
}
