using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Globalization;

/// <summary>
/// Specifies the type of dimension represented, such as length, width, or height.
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
