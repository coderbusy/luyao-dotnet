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
    /// <returns>包含所有 WMI 数据的嵌套字典，键为WMI类名</returns>
    private static Dictionary<String, Dictionary<String, String>> ReadWmicBatch()
    {
        var result = new Dictionary<String, Dictionary<String, String>>(StringComparer.OrdinalIgnoreCase);
        
        // 读取计算机系统产品信息
        AddWmicData(result, "csproduct", ReadWmic("csproduct", "Name", "Vendor"));
        
        // 读取操作系统信息
        AddWmicData(result, "os", ReadWmic("os", "Caption", "Version"));
        
        // 读取磁盘驱动器信息
        AddWmicData(result, "diskdrive", ReadWmic("diskdrive where mediatype=\"Fixed hard disk media\"", "SerialNumber"));
        
        // 读取BIOS信息
        AddWmicData(result, "bios", ReadWmic("bios", "SerialNumber"));
        
        // 读取主板信息
        AddWmicData(result, "baseboard", ReadWmic("baseboard", "SerialNumber"));
        
        return result;
    }

    /// <summary>将WMIC数据添加到结果字典</summary>
    private static void AddWmicData(Dictionary<String, Dictionary<String, String>> result, String key, Dictionary<String, String> data)
    {
        if (data.Count > 0)
            result[key] = data;
    }
    
    /// <summary>通过WMIC命令读取信息</summary>
    /// <param name="type">WMI类型</param>
    /// <param name="keys">查询字段</param>
    /// <returns>解析后的字典</returns>
    private static Dictionary<String, String> ReadWmic(String type, params String[] keys)
    {
        const Int32 WmicTimeoutMilliseconds = 5000;
        
        var rawData = new Dictionary<String, List<String>>(StringComparer.OrdinalIgnoreCase);
        var result = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var output = ExecuteWmicCommand(type, keys, WmicTimeoutMilliseconds);
            if (String.IsNullOrEmpty(output))
                return result;

            ParseWmicOutput(output, rawData);
            ConsolidateWmicData(rawData, result);
        }
        catch
        {
            // 忽略错误，返回空结果
        }

        return result;
    }

    /// <summary>执行WMIC命令并返回输出</summary>
    private static String? ExecuteWmicCommand(String type, String[] keys, Int32 timeoutMs)
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
        if (process == null)
            return null;

        var output = process.StandardOutput.ReadToEnd();
        
        if (!process.WaitForExit(timeoutMs))
        {
            try { process.Kill(); } catch { }
            return null;
        }

        return output;
    }

    /// <summary>解析WMIC输出为键值对</summary>
    private static void ParseWmicOutput(String output, Dictionary<String, List<String>> rawData)
    {
        var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        
        foreach (var line in lines)
        {
            var parts = line.Split('=');
            if (parts.Length < 2)
                continue;

            var key = parts[0].Trim();
            var value = Clean(parts[1].Trim()) ?? "";

            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                continue;

            if (!rawData.TryGetValue(key, out var list))
                rawData[key] = list = new List<String>();

            list.Add(value);
        }
    }

    /// <summary>合并多个值的WMIC数据（如多个磁盘）</summary>
    private static void ConsolidateWmicData(Dictionary<String, List<String>> rawData, Dictionary<String, String> result)
    {
        foreach (var item in rawData)
        {
            // 排序以保证一致性，用逗号连接多个值
            result[item.Key] = String.Join(",", item.Value.OrderBy(e => e));
        }
    }
    #endregion
}
