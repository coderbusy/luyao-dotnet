using System;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Xsl;

namespace LuYao.Xml;

class TranslatableHelper : ConcurrentDictionary<Type, XslCompiledTransform>
{
    private TranslatableHelper()
    {

    }
    private static readonly TranslatableHelper Instance = new();
    public static XslCompiledTransform Get<T>()
    {
        return Instance.GetOrAdd(typeof(T), type =>
        {
            var xsltFileNames = new string[] { type.FullName + ".xslt", type.FullName + ".xsl" };
            foreach (var name in xsltFileNames)
            {
                using var ms = type.Assembly.GetManifestResourceStream(name);
                if (ms == null) continue;
                var ret = new XslCompiledTransform();
                using (var reader = XmlReader.Create(ms))
                {
                    ret.Load(reader);
                }
                return ret;
            }
            throw new InvalidOperationException("没有找到与类型名相同的 xslt或xsl 文件，请确保文件目录与类型的命名空间一致且已经被设置为“嵌入的资源”。");
        });
    }
}
