using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Globalization;

/// <summary>
/// 指定所表示尺寸的类型，例如长度、宽度或高度。
/// </summary>
public enum DimensionKind
{
    /// <summary>
    /// 未指定
    /// </summary>
    Unspecified,
    /// <summary>
    /// 长度
    /// </summary>
    Length,
    /// <summary>
    /// 宽度
    /// </summary>
    Width,
    /// <summary>
    /// 高度
    /// </summary>
    Height,
}
