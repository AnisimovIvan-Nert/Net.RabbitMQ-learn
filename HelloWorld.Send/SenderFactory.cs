using System.Text;

namespace HelloWorld.Send;

public static class SenderFactory
{
    public static async ValueTask<MessageSender> CreateAsync(string connectionString, string queue, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var sender = new MessageSender(connectionString, queue, encoding);
        await sender.InitializeAsync();
        return sender;
    }
}