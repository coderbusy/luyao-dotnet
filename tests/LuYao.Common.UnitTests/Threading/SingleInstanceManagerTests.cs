using System.IO.Pipes;

namespace LuYao.Threading;

[TestClass]
public class SingleInstanceManagerTests
{
    [TestMethod]
    public void ReleaseLock_WhenNoLockHeld_ShouldNotThrow()
    {
        // Arrange & Act: calling ReleaseLock without holding a lock should be safe
        SingleInstanceManager.ReleaseLock();
    }

    [TestMethod]
    public void ReleaseLock_CalledTwice_ShouldNotThrow()
    {
        // Act & Assert: calling ReleaseLock multiple times should be safe
        SingleInstanceManager.ReleaseLock();
        SingleInstanceManager.ReleaseLock();
    }

    [TestMethod]
    public void ActivateWindowRequested_SubscribeAndUnsubscribe_ShouldNotThrow()
    {
        // Arrange
        EventHandler handler = (_, _) => { };

        // Act & Assert: subscribing and unsubscribing should not throw
        SingleInstanceManager.ActivateWindowRequested += handler;
        SingleInstanceManager.ActivateWindowRequested -= handler;
    }

    [TestMethod]
    public async Task NamedPipe_ClientCanConnectAndSendCommand()
    {
        // Arrange: verify that a named pipe server/client pair works for the ActivateWindow command
        const string pipeName = "LuYaoSI_unit_test_pipe_connect";
        const string expectedCommand = "ActivateWindow";
        string? receivedCommand = null;
        var tcs = new TaskCompletionSource<string?>();

        var serverTask = Task.Run(async () =>
        {
            using var server = new NamedPipeServerStream(pipeName);
            await server.WaitForConnectionAsync();
            using var reader = new StreamReader(server);
            tcs.TrySetResult(await reader.ReadLineAsync());
        });

        // Act: connect as a client and write the command
        using (var client = new NamedPipeClientStream(pipeName))
        {
            client.Connect(2000);
            using var writer = new StreamWriter(client);
            writer.WriteLine(expectedCommand);
            writer.Flush();
        }

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(3000));
        Assert.AreEqual(tcs.Task, completed, "Server should have received the command within timeout.");
        receivedCommand = tcs.Task.Result;

        // Assert
        Assert.AreEqual(expectedCommand, receivedCommand);
    }
}
