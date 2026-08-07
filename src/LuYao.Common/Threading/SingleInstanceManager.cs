using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Threading;

/// <summary>
/// 提供单实例进程管理，确保应用程序每次只有一个实例运行，
/// 并允许新实例通过命名管道通知现有实例激活窗口。
/// </summary>
public static class SingleInstanceManager
{
    private const string Command_ActivateWindow = "ActivateWindow";
    private static Mutex? _processLock;
    private static bool _hasLock;

    /// <summary>
    /// 当应用程序的另一个实例请求当前实例激活主窗口时触发。
    /// </summary>
    public static event EventHandler? ActivateWindowRequested;

    /// <summary>
    /// 确保应用程序只有一个实例正在运行。
    /// 如果当前进程是第一个实例，则开始通过命名管道侦听激活请求；
    /// 如果已有实例正在运行，则向现有实例发送激活请求并终止当前进程。
    /// </summary>
    public static void EnsureSingleInstance()
    {
        var uid = GetUid();
        var mutexName = $"Global\\LuYao.SingleInstance[{uid}]";
        var pipeName = $"LuYaoSI{uid}";
        _processLock = new Mutex(false, mutexName, out _hasLock);
        if (_hasLock)
        {
            Task.Factory.StartNew(() => ServerThread(pipeName), TaskCreationOptions.LongRunning);
        }
        else
        {
            RequestActivateWindow(pipeName);
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// 释放当前进程持有的单实例锁。
    /// </summary>
    /// <remarks>谨慎使用——释放锁允许另一个实例成为主实例。</remarks>
    public static void ReleaseLock()
    {
        if (_processLock != null && _hasLock)
        {
            _processLock.Dispose();
            _hasLock = false;
        }
    }

    private static string GetUid()
    {
#if NET6_0_OR_GREATER
        var path = Environment.ProcessPath ?? AppContext.BaseDirectory;
#elif NET45
        var path = AppDomain.CurrentDomain.BaseDirectory;
#else
        var path = AppContext.BaseDirectory;
#endif
        var bytes = Encoding.UTF8.GetBytes(path);
        using (var md5 = MD5.Create())
        {
            bytes = md5.ComputeHash(bytes);
        }
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    private static async Task ServerThread(string pipeName)
    {
        while (true)
        {
            try
            {
                using (var server = new NamedPipeServerStream(pipeName))
                {
#if NETFRAMEWORK
                    await Task.Run(() => server.WaitForConnection());
#else
                    await server.WaitForConnectionAsync();
#endif
                    using (var reader = new StreamReader(server))
                    {
                        var command = await reader.ReadLineAsync();
                        if (command == Command_ActivateWindow)
                        {
                            try
                            {
                                ActivateWindowRequested?.Invoke(null, EventArgs.Empty);
                            }
                            catch
                            {
                                // 防止订阅者异常终止服务器循环。
                            }
                        }
                    }
                }
            }
            catch
            {
                // 防止管道 I/O 错误终止服务器循环。
            }
        }
    }

    private static void RequestActivateWindow(string pipeName)
    {
        using (var client = new NamedPipeClientStream(pipeName))
        {
            client.Connect(1000);
            using (var writer = new StreamWriter(client))
            {
                writer.WriteLine(Command_ActivateWindow);
                writer.Flush();
            }
        }
    }
}
