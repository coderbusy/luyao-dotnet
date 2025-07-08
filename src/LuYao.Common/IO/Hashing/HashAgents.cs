namespace LuYao.IO.Hashing;

/// <summary>
/// 提供常用哈希算法的静态实例。
/// </summary>
public static class HashAgents
{
    /// <summary>
    /// CRC32 哈希算法实例。
    /// </summary>
    public static HashAgent CRC32 { get; } = new HashAgent(() => new Crc32());
    /// <summary>
    /// CRC64 哈希算法实例。
    /// </summary>
    public static HashAgent CRC64 { get; } = new HashAgent(() => new Crc64Ecma182());
    /// <summary>
    /// MD5 哈希算法实例。
    /// </summary>
    public static HashAgent MD5 { get; } = new HashAgent(() => System.Security.Cryptography.MD5.Create());
    /// <summary>
    /// SHA1 哈希算法实例。
    /// </summary>
    public static HashAgent SHA1 { get; } = new HashAgent(() => System.Security.Cryptography.SHA1.Create());
    /// <summary>
    /// SHA256 哈希算法实例。
    /// </summary>
    public static HashAgent SHA256 { get; } = new HashAgent(() => System.Security.Cryptography.SHA256.Create());
    /// <summary>
    /// SHA384 哈希算法实例。
    /// </summary>
    public static HashAgent SHA384 { get; } = new HashAgent(() => System.Security.Cryptography.SHA384.Create());
    /// <summary>
    /// SHA512 哈希算法实例。
    /// </summary>
    public static HashAgent SHA512 { get; } = new HashAgent(() => System.Security.Cryptography.SHA512.Create());
}
