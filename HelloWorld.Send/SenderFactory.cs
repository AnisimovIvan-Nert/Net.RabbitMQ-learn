using System.Text;
using Base;
using Base.Sender;

namespace HelloWorld.Send;

public static class SenderFactory
{
    public static async ValueTask<MessageSender> CreateAsync(
        string connectionString, 
        string queue,
        bool durable = false,
        Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var connectionOptions = new ConnectionOptions(connectionString, queue, durable);
        var senderOptions = new SenderOptions(connectionOptions);

        var sender = new MessageSender(senderOptions, encoding);
        await sender.InitializeAsync();
        return sender;
    }
}