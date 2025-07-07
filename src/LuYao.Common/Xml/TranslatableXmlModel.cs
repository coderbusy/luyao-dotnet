using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LuYao.Xml;

/// <summary>
/// 可转换的 XML 模型基类，提供 XML 文档的转换和反序列化功能
/// </summary>
/// <typeparam name="T">继承自 TranslatableXmlModel 的具体类型</typeparam>
/// <remarks>
/// 此类提供以下功能：
/// 1. 通过 XSLT 转换 XML 文档
/// 2. 自动反序列化 XML
/// 3. 支持转换后的回调处理
/// </remarks>
[Serializable]
public abstract class TranslatableXmlModel<T> where T : TranslatableXmlModel<T>
{
    private static readonly XmlSerializer XmlSerializer;

    static TranslatableXmlModel()
    {
        XmlSerializer = new XmlSerializer(typeof(T));
    }

    /// <summary>
    /// 在 XML 转换完成后触发的回调方法
    /// </summary>
    protected virtual void OnTransformed() { }

    /// <summary>
    /// 将 XML 字符串转换为指定的模型对象
    /// </summary>
    /// <param name="xml">要转换的 XML 字符串</param>
    /// <returns>转换后的类型为 T 的对象实例</returns>
    public static T Transform(string xml)
    {
        var sb = new StringBuilder();
        using (var sw = new StringWriter(sb))
        {
            using (var xmlWriter = new XmlTextWriter(sw) { Formatting = Formatting.Indented })
            {
                using (var txtReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(txtReader))
                {
                    TranslatableHelper.Get<T>().Transform(xmlReader, xmlWriter);
                }
            }
        }
        var output = sb.ToString();
        using (var reader = new StringReader(output))
        {
            var ret = (T)XmlSerializer.Deserialize(reader);
            ret.OnTransformed();
            return ret;
        }
    }
}
