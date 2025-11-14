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

        // 一次性通过 WMIC 批量获取所有信息以提高性能
        var wmicNeeded = String.IsNullOrEmpty(Vendor) || String.IsNullOrEmpty(Product) || 
                         String.IsNullOrEmpty(OSName) || String.IsNullOrEmpty(Serial) || 
                         String.IsNullOrEmpty(Board) || String.IsNullOrEmpty(DiskID);
        
        if (wmicNeeded)
        {
            var wmicData = ReadWmicBatch();
            
            // 处理 csproduct 数据
            if (wmicData.TryGetValue("csproduct", out var csproduct))
            {
                if (csproduct.TryGetValue("Name", out str) && !String.IsNullOrEmpty(str) && String.IsNullOrEmpty(Product)) 
                    Product = str;
                if (csproduct.TryGetValue("Vendor", out str) && !String.IsNullOrEmpty(str) && String.IsNullOrEmpty(Vendor)) 
                    Vendor = str;
            }
            
            // 处理 OS 数据
            if (wmicData.TryGetValue("os", out var os))
            {
                if (String.IsNullOrEmpty(OSName) && os.TryGetValue("Caption", out str)) 
                    OSName = str.Replace("Microsoft", "").Trim();
                if (os.TryGetValue("Version", out str)) 
                    OSVersion = str;
            }
            
            // 处理磁盘数据
            if (wmicData.TryGetValue("diskdrive", out var disk) && disk.TryGetValue("SerialNumber", out str))
                DiskID = str?.Trim();
            
            // 处理 BIOS 序列号
            if (wmicData.TryGetValue("bios", out var bios) && bios.TryGetValue("SerialNumber", out str) && 
                !str.Equals("System Serial Number", StringComparison.OrdinalIgnoreCase))
                Serial = str?.Trim();
            
            // 处理主板序列号
            if (wmicData.TryGetValue("baseboard", out var board) && board.TryGetValue("SerialNumber", out str))
                Board = str?.Trim();
        }

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
    /// <summary>批量读取 WMIC 信息以提高性能</summary>
    /// <returns>包含所有 WMI 数据的嵌套字典</returns>
    private static Dictionary<String, Dictionary<String, String>> ReadWmicBatch()
    {
        var result = new Dictionary<String, Dictionary<String, String>>(StringComparer.OrdinalIgnoreCase);
        
        // csproduct
        var data = ReadWmic("csproduct", "Name", "Vendor");
        if (data.Count > 0) result["csproduct"] = data;
        
        // os
        data = ReadWmic("os", "Caption", "Version");
        if (data.Count > 0) result["os"] = data;
        
        // diskdrive
        data = ReadWmic("diskdrive where mediatype=\"Fixed hard disk media\"", "SerialNumber");
        if (data.Count > 0) result["diskdrive"] = data;
        
        // bios
        data = ReadWmic("bios", "SerialNumber");
        if (data.Count > 0) result["bios"] = data;
        
        // baseboard
        data = ReadWmic("baseboard", "SerialNumber");
        if (data.Count > 0) result["baseboard"] = data;
        
        return result;
    }
    
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
                // 添加超时以避免挂起（5秒）
                if (!process.WaitForExit(5000))
                {
                    try { process.Kill(); } catch { }
                    return dic2;
                }

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
