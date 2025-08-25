using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.IO.Hashing;

/// <summary>
/// 哈希算法代理基类，提供多种数据类型的哈希计算方法。
/// </summary>
public class HashAgent
{
    private Func<HashAlgorithm> _hashAlgorithmFactory;

    /// <summary>
    /// 初始化 HashAgent 类的新实例。
    /// </summary>
    /// <param name="hashAlgorithmFactory">用于创建哈希算法实例的工厂方法。</param>
    public HashAgent(Func<HashAlgorithm> hashAlgorithmFactory)
    {
        _hashAlgorithmFactory = hashAlgorithmFactory;
    }


    /// <summary>
    /// 计算字符串的哈希值，使用 UTF-8 编码。
    /// </summary>
    /// <param name="text">要计算哈希的字符串。</param>
    /// <returns>哈希值的十六进制字符串表示。</returns>
    public string Hash(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        return Hash(bytes);
    }

    /// <summary>
    /// 计算字节数组的哈希值。
    /// </summary>
    /// <param name="data">要计算哈希的字节数组。</param>
    /// <returns>哈希值的十六进制字符串表示。</returns>
    public string Hash(byte[] data)
    {
        using (var hash = CreateHashAlgorithm())
        {
            var bytes = hash.ComputeHash(data);
            return ToString(bytes);
        }
    }

    /// <summary>
    /// 计算流的哈希值。
    /// </summary>
    /// <param name="stream">要计算哈希的流。</param>
    /// <returns>哈希值的十六进制字符串表示。</returns>
    public string Hash(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream), "Stream cannot be null.");
        using (var hash = CreateHashAlgorithm())
        {
            var bytes = hash.ComputeHash(stream);
            return ToString(bytes);
        }
    }

    /// <summary>
    /// 计算文件的哈希值。
    /// </summary>
    /// <param name="filePath">要计算哈希的文件路径。</param>
    /// <returns>哈希值的十六进制字符串表示。</returns>
    public string HashFile(string filePath)
    {
        using (var fs = File.OpenRead(filePath)) return Hash(fs);
    }

    /// <summary>
    /// 创建哈希算法实例。
    /// </summary>
    /// <returns>哈希算法实例。</returns>
    protected HashAlgorithm CreateHashAlgorithm() => _hashAlgorithmFactory.Invoke();

    private static string ToString(byte[] data)
    {
        var sb = new StringBuilder();
        if (data != null && data.Any())
        {
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
        }
        return sb.ToString();
    }
}
