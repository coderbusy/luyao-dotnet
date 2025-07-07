using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LuYao.Xml;


[Serializable]
public abstract class TranslatableXmlModel<T> where T : TranslatableXmlModel<T>
{
    private static readonly XmlSerializer XmlSerializer;

    static TranslatableXmlModel()
    {
        XmlSerializer = new XmlSerializer(typeof(T));
    }

    protected virtual void OnTransformed() { }

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
