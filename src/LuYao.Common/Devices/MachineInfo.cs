using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Devices;

public partial class MachineInfo
{
    public static MachineInfo Get()
    {
        var info = new MachineInfo();
        info.Reload();
        return info;
    }
    /// <summary>系统名称</summary>
    public String? OSName { get; set; }

    /// <summary>系统版本</summary>
    public String? OSVersion { get; set; }

    /// <summary>产品名称</summary>
    public String? Product { get; set; }

    /// <summary>制造商</summary>
    public String? Vendor { get; set; }

    /// <summary>处理器型号</summary>
    public String? Processor { get; set; }
    /// <summary>计算机序列号。适用于品牌机，跟笔记本标签显示一致</summary>
    public String? Serial { get; set; }

    /// <summary>主板。序列号或家族信息</summary>
    public String? Board { get; set; }

    /// <summary>磁盘序列号</summary>
    public String? DiskID { get; set; }
    private void Reset()
    {
        this.OSName = Environment.OSVersion.Platform.ToString();
        this.OSVersion = Environment.OSVersion.VersionString;
    }
    public void Reload()
    {
        this.Reset();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            MachineInfoWindows.Reload(this);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            MachineInfoLinux.Reload(this);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            MachineInfoMacOS.Reload(this);
        }
    }

    private static class MachineInfoWindows
    {
        public static void Reload(MachineInfo info)
        {
            throw new NotImplementedException();
        }
    }
    private static class MachineInfoLinux
    {
        public static void Reload(MachineInfo info)
        {
            throw new NotImplementedException();
        }
    }
    private static class MachineInfoMacOS
    {
        public static void Reload(MachineInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
