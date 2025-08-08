using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data.Models;

/// <summary>
/// 定义记录加载时用于标识字段的键类型。
/// </summary>
/// <remarks>
/// 此枚举用于 <see cref="LuYao.Data.RecordLoadAdapter"/> 中，指示在加载记录数据时
/// 如何识别和访问字段：通过字段名称或通过字段在记录中的索引位置。
/// </remarks>
public enum RecordLoadKeyKind
{
    /// <summary>
    /// 通过字段名称识别和访问记录中的字段。
    /// </summary>
    /// <remarks>
    /// 当使用此模式时，加载适配器将使用 <see cref="LuYao.Data.RecordLoadAdapter.Name"/> 
    /// 属性来查找和匹配记录中的列。这种方式提供了更好的可读性和灵活性，
    /// 但在性能上可能略逊于基于索引的访问方式。
    /// </remarks>
    Name,

    /// <summary>
    /// 通过字段索引位置识别和访问记录中的字段。
    /// </summary>
    /// <remarks>
    /// 当使用此模式时，加载适配器将使用 <see cref="LuYao.Data.RecordLoadAdapter.Index"/> 
    /// 属性来定位记录中的列。这种方式通常具有更好的性能，
    /// 但要求调用方了解字段在记录中的确切位置。
    /// </remarks>
    Index
}