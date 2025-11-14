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

            // 尝试获取供应商信息 (通常是 Apple)
            Vendor = "Apple";
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
