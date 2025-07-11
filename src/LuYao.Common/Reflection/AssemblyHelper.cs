using System;
using System.Reflection;

namespace LuYao.Reflection;

/// <summary>
/// 提供与程序集相关的辅助方法。
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    /// 尝试获取指定程序集的构建时间。
    /// </summary>
    /// <param name="path">程序集文件的路径。</param>
    /// <param name="buildTime">输出参数，用于返回构建时间。</param>
    /// <returns>如果成功获取构建时间，则返回 true；否则返回 false。</returns>
    public static bool TryGetBuildTime(string path, out DateTime buildTime)
    {
        buildTime = new DateTime(2000, 1, 1);
        try
        {
            var assemblyName = AssemblyName.GetAssemblyName(path);
            if (assemblyName.Version != null)
            {
                Version version = assemblyName.Version;
                buildTime = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            }
        }
        catch
        {
            buildTime = DateTime.MinValue;
        }
        return buildTime != DateTime.MinValue;
    }
}
