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
            var output = ExecuteCommand("system_profiler", "SPHardwareDataType SPSoftwareDataType");
            if (!String.IsNullOrEmpty(output))
            {
                ParseSystemProfilerOutput(output);
            }

            // macOS 设备供应商总是 Apple
            Vendor = "Apple";
        }
        catch
        {
            // 忽略错误
        }

        // 使用后备方案获取OS名称
        if (String.IsNullOrEmpty(OSName))
        {
            OSName = GetFallbackOSName();
        }
    }

    /// <summary>解析 system_profiler 命令输出</summary>
    private void ParseSystemProfilerOutput(String output)
    {
        var lines = output.Split('\n');
        var currentSection = ProfilerSection.None;
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            // 检测区域切换
            currentSection = DetectSection(line, currentSection);
            
            // 根据当前区域解析信息
            switch (currentSection)
            {
                case ProfilerSection.Hardware:
                    ParseHardwareInfo(trimmed);
                    break;
                case ProfilerSection.Software:
                    ParseSoftwareInfo(trimmed);
                    break;
            }
        }
    }

    /// <summary>检测 system_profiler 输出的区域</summary>
    private static ProfilerSection DetectSection(String line, ProfilerSection current)
    {
        if (line.Contains("Hardware:"))
            return ProfilerSection.Hardware;
        if (line.Contains("Software:"))
            return ProfilerSection.Software;
        return current;
    }

    /// <summary>解析硬件信息行</summary>
    private void ParseHardwareInfo(String trimmed)
    {
        if (TryExtractValue(trimmed, "Model Name:", out var modelName))
            Product = modelName;
        else if (String.IsNullOrEmpty(Product) && TryExtractValue(trimmed, "Model Identifier:", out var modelId))
            Product = modelId;
        else if (TryExtractValue(trimmed, "Processor Name:", out var processorName))
            Processor = processorName;
        else if (String.IsNullOrEmpty(Processor) && TryExtractValue(trimmed, "Chip:", out var chip))
            Processor = chip;
        else if (TryExtractValue(trimmed, "Serial Number (system):", out var serial))
            Serial = serial;
    }

    /// <summary>解析软件信息行</summary>
    private void ParseSoftwareInfo(String trimmed)
    {
        if (TryExtractValue(trimmed, "System Version:", out var systemVersion))
        {
            OSName = systemVersion;
            OSVersion = ExtractVersionNumber(systemVersion);
        }
    }

    /// <summary>尝试从行中提取键值对</summary>
    private static Boolean TryExtractValue(String line, String prefix, out String value)
    {
        value = "";
        if (!line.StartsWith(prefix))
            return false;

        value = line.Substring(prefix.Length).Trim();
        return true;
    }

    /// <summary>从系统版本字符串中提取版本号</summary>
    private static String ExtractVersionNumber(String systemVersion)
    {
        var versionStart = systemVersion.IndexOf('(');
        if (versionStart > 0)
            return systemVersion.Substring(0, versionStart).Trim();
        return systemVersion;
    }

    /// <summary>获取后备OS名称</summary>
    private static String GetFallbackOSName()
    {
#if NETFRAMEWORK
        return Environment.OSVersion.Platform.ToString();
#else
        return RuntimeInformation.OSDescription;
#endif
    }

    /// <summary>system_profiler 输出区域</summary>
    private enum ProfilerSection
    {
        None,
        Hardware,
        Software
    }

    #region macOS辅助方法
    /// <summary>执行命令并返回标准输出</summary>
    /// <param name="command">命令名称</param>
    /// <param name="arguments">命令参数</param>
    /// <returns>命令输出，失败则返回null</returns>
    private static String? ExecuteCommand(String command, String arguments)
    {
        const Int32 SystemProfilerTimeoutMs = 10000; // system_profiler 较慢，需要更长超时
        
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
            if (process == null)
                return null;

            var output = process.StandardOutput.ReadToEnd();
            
            // 等待进程结束或超时
            if (!process.WaitForExit(SystemProfilerTimeoutMs))
            {
                try { process.Kill(); } catch { }
                return null;
            }
            
            return output;
        }
        catch
        {
            // 忽略错误
        }

        return null;
    }
    #endregion
}
