using System.Text;

namespace HelloWorld.Receive;

public static class ReceiverFactory
{
    public static async ValueTask<MessageReceiver> CreateAsync(string connectionString, string queue, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var receiver = new MessageReceiver(connectionString, queue, encoding);
        await receiver.InitializeAsync();
        return receiver;
    }
}