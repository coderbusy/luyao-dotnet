using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Threading;

/// <summary>
/// Provides single-instance process management. Ensures only one instance of the
/// application runs at a time, and allows the new instance to signal the existing
/// one to activate its window via a named pipe.
/// </summary>
public static class SingleInstanceManager
{
    private const string Command_ActivateWindow = "ActivateWindow";
    private static Mutex? _processLock;
    private static bool _hasLock;

    /// <summary>
    /// Raised when another instance of the application requests that the current
    /// instance activates its main window.
    /// </summary>
    public static event EventHandler? ActivateWindowRequested;

    /// <summary>
    /// Ensures that only one instance of the application is running.
    /// If this is the first instance, it starts listening on a named pipe for
    /// activation requests. If another instance is already running, it sends an
    /// activation request to the existing instance and terminates the current process.
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
    /// Releases the single-instance lock held by the current process.
    /// </summary>
    /// <remarks>Use with caution — releasing the lock allows another instance to become the primary instance.</remarks>
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
                                // Prevent a subscriber exception from terminating the server loop.
                            }
                        }
                    }
                }
            }
            catch
            {
                // Prevent a pipe I/O error from terminating the server loop.
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
