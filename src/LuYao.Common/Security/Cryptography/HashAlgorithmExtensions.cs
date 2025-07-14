using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Security.Cryptography;

/// <summary>
/// 提供扩展方法以异步计算哈希值。
/// </summary>
public static class HashAlgorithmExtensions
{
    /// <summary>
    /// 异步计算指定流的哈希值。
    /// </summary>
    /// <param name="hashAlgorithm">哈希算法实例。</param>
    /// <param name="stream">要计算哈希值的输入流。</param>
    /// <param name="cancellationToken">用于取消操作的令牌。</param>
    /// <param name="progress">用于报告读取进度的进度实例。</param>
    /// <param name="bufferSize">缓冲区大小（以字节为单位）。</param>
    /// <returns>计算得到的哈希值字节数组。</returns>
    /// <exception cref="OperationCanceledException">当操作被取消时抛出。</exception>
    public static async Task<byte[]> ComputeHashAsync(this HashAlgorithm hashAlgorithm, Stream stream,
        int bufferSize = 1024 * 1024, IProgress<long>? progress = null, CancellationToken cancellationToken = default(CancellationToken)
    )
    {
        if (hashAlgorithm == null) throw new ArgumentNullException(nameof(hashAlgorithm));
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        byte[] readAheadBuffer, buffer;
        int readAheadBytesRead, bytesRead;
        long totalBytesRead = 0;
        readAheadBuffer = new byte[bufferSize];
        readAheadBytesRead = await stream.ReadAsync(readAheadBuffer, 0, readAheadBuffer.Length, cancellationToken);
        totalBytesRead += readAheadBytesRead;
        do
        {
            bytesRead = readAheadBytesRead;
            buffer = readAheadBuffer;
            readAheadBuffer = new byte[bufferSize];
            readAheadBytesRead = await stream.ReadAsync(readAheadBuffer, 0, readAheadBuffer.Length, cancellationToken);
            totalBytesRead += readAheadBytesRead;

            if (readAheadBytesRead == 0)
                hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
            else
                hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);

            if (progress != null) progress.Report(totalBytesRead);
            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
        } while (readAheadBytesRead != 0);

        return hashAlgorithm.Hash ?? Arrays.Empty<byte>();
    }
}
