using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

namespace LuYao.Devices;

/// <summary>
/// MachineInfo 的 macOS 平台实现部分
/// </summary>
public partial class MachineInfo
{
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("macos")]
#endif
    private void LoadMacInfo()
    {
        try
        {
            // 获取硬件信息
            var hardware = ExecuteCommand("system_profiler", "SPHardwareDataType");
            if (!String.IsNullOrEmpty(hardware))
            {
                var lines = hardware.Split('\n');
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    
                    if (trimmed.StartsWith("Model Name:"))
                        Product = trimmed.Substring("Model Name:".Length).Trim();
                    else if (trimmed.StartsWith("Model Identifier:") && String.IsNullOrEmpty(Product))
                        Product = trimmed.Substring("Model Identifier:".Length).Trim();
                    else if (trimmed.StartsWith("Processor Name:"))
                        Processor = trimmed.Substring("Processor Name:".Length).Trim();
                    else if (trimmed.StartsWith("Chip:") && String.IsNullOrEmpty(Processor))
                        Processor = trimmed.Substring("Chip:".Length).Trim();
                    else if (trimmed.StartsWith("Serial Number (system):"))
                        Serial = trimmed.Substring("Serial Number (system):".Length).Trim();
                    else if (trimmed.StartsWith("Hardware UUID:"))
                        UUID = trimmed.Substring("Hardware UUID:".Length).Trim();
                    else if (trimmed.StartsWith("Memory:"))
                    {
                        var memStr = trimmed.Substring("Memory:".Length).Trim();
                        // Parse memory like "16 GB" or "8 GB"
                        var parts = memStr.Split(' ');
                        if (parts.Length >= 2 && Double.TryParse(parts[0], out var memValue))
                        {
                            if (parts[1].Equals("GB", StringComparison.OrdinalIgnoreCase))
                                Memory = (UInt64)(memValue * 1024 * 1024 * 1024);
                            else if (parts[1].Equals("MB", StringComparison.OrdinalIgnoreCase))
                                Memory = (UInt64)(memValue * 1024 * 1024);
                        }
                    }
                }
            }

            // 获取软件信息
            var software = ExecuteCommand("system_profiler", "SPSoftwareDataType");
            if (!String.IsNullOrEmpty(software))
            {
                var lines = software.Split('\n');
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    
                    if (trimmed.StartsWith("System Version:"))
                    {
                        OSName = trimmed.Substring("System Version:".Length).Trim();
                        // Extract version number
                        var versionStart = OSName.IndexOf('(');
                        if (versionStart > 0)
                        {
                            OSVersion = OSName.Substring(0, versionStart).Trim();
                        }
                    }
                }
            }

            // 获取 UUID（如果上面没有获取到）
            if (String.IsNullOrEmpty(UUID))
            {
                UUID = ExecuteCommand("ioreg", "-rd1 -c IOPlatformExpertDevice | grep IOPlatformUUID")?.Trim();
                if (!String.IsNullOrEmpty(UUID))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(UUID, @"""([A-F0-9-]+)""");
                    if (match.Success)
                        UUID = match.Groups[1].Value;
                }
            }
        }
        catch
        {
            // 忽略错误
        }

        if (String.IsNullOrEmpty(OSName))
        {
#if NETFRAMEWORK
            OSName = Environment.OSVersion.Platform.ToString();
#else
            OSName = RuntimeInformation.OSDescription;
#endif
        }
    }

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("macos")]
#endif
    private void RefreshMacOS()
    {
        // 获取内存信息
        try
        {
            var vmStat = ExecuteCommand("vm_stat", "");
            if (!String.IsNullOrEmpty(vmStat))
            {
                var pageSize = 4096UL; // macOS typical page size
                var lines = vmStat.Split('\n');
                
                UInt64 free = 0, active = 0, inactive = 0, wired = 0;
                
                foreach (var line in lines)
                {
                    if (line.Contains("page size of"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+)");
                        if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var ps))
                            pageSize = ps;
                    }
                    else if (line.StartsWith("Pages free:"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+)");
                        if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var pages))
                            free = pages * pageSize;
                    }
                    else if (line.StartsWith("Pages active:"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+)");
                        if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var pages))
                            active = pages * pageSize;
                    }
                    else if (line.StartsWith("Pages inactive:"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+)");
                        if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var pages))
                            inactive = pages * pageSize;
                    }
                    else if (line.StartsWith("Pages wired down:"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+)");
                        if (match.Success && UInt64.TryParse(match.Groups[1].Value, out var pages))
                            wired = pages * pageSize;
                    }
                }

                FreeMemory = free;
                AvailableMemory = free + inactive;
            }
        }
        catch
        {
            // 忽略错误
        }

        // 获取CPU使用率
        try
        {
            var top = ExecuteCommand("top", "-l 1 -n 0");
            if (!String.IsNullOrEmpty(top))
            {
                var lines = top.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("CPU usage:"))
                    {
                        // Parse "CPU usage: 3.57% user, 14.28% sys, 82.14% idle"
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+\.?\d*)%\s+idle");
                        if (match.Success && Double.TryParse(match.Groups[1].Value, out var idle))
                        {
                            CpuRate = (100.0 - idle) / 100.0;
                        }
                        break;
                    }
                }
            }
        }
        catch
        {
            // 忽略错误
        }
    }

    #region macOS辅助方法
    private static String? ExecuteCommand(String command, String arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
        catch
        {
            // 忽略错误
        }

        return null;
    }
    #endregion
}
