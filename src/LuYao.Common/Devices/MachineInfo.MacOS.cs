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
            // 一次性获取硬件和软件信息以提高性能
            // 使用 xml 格式输出更快且更结构化
            var output = ExecuteCommand("system_profiler", "SPHardwareDataType SPSoftwareDataType");
            if (!String.IsNullOrEmpty(output))
            {
                var lines = output.Split('\n');
                var inHardware = false;
                var inSoftware = false;
                
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    
                    // 检测区域切换
                    if (line.Contains("Hardware:"))
                    {
                        inHardware = true;
                        inSoftware = false;
                        continue;
                    }
                    else if (line.Contains("Software:"))
                    {
                        inHardware = false;
                        inSoftware = true;
                        continue;
                    }
                    
                    // 解析硬件信息
                    if (inHardware)
                    {
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
                    // 解析软件信息
                    else if (inSoftware && trimmed.StartsWith("System Version:"))
                    {
                        OSName = trimmed.Substring("System Version:".Length).Trim();
                        // 提取版本号
                        var versionStart = OSName.IndexOf('(');
                        if (versionStart > 0)
                        {
                            OSVersion = OSName.Substring(0, versionStart).Trim();
                        }
                    }
                }
            }

            // 供应商信息（总是 Apple）
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
                // 添加超时以避免挂起（10秒，system_profiler 较慢）
                if (!process.WaitForExit(10000))
                {
                    try { process.Kill(); } catch { }
                    return null;
                }
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
