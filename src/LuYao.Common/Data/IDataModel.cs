using System;

namespace LuYao.Data;

/// <summary>模型数据接口，支持索引器读写属性</summary>
public interface IDataModel
{
    /// <summary>设置 或 获取 数据项</summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Object? this[String key] { get; set; }
}
