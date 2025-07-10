using System;
using System.Collections.Concurrent;
using System.IO;

namespace LuYao.Text.Json;

class TranslatableHelper : ConcurrentDictionary<Type, string>
{
    private TranslatableHelper()
    {

    }

    private static readonly TranslatableHelper Instance = new();

    public static string Get<T>()
    {
        return Instance.GetOrAdd(typeof(T), type =>
        {
            var xsltFileNames = new[] { type.FullName + ".js" };
            foreach (var name in xsltFileNames)
            {
                using var ms = type.Assembly.GetManifestResourceStream(name);
                if (ms == null) continue;
                using var sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }

            throw new FileNotFoundException("没有找到与类型名相同的 js 文件，请确保文件目录与类型的命名空间一致且已经被设置为”嵌入的资源“。");
        });
    }
}