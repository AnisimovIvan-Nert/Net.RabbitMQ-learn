using System.Text;
using Base;
using Base.Receiver;

namespace HelloWorld.Receive;

public static class ReceiverFactory
{
    public static async ValueTask<MessageReceiver> CreateAsync(
        string connectionString, 
        string queue, 
        bool durable = false,
        bool autoAcknowledgment = true, 
        Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var connectionOptions = new ConnectionOptions(connectionString, queue, durable);
        var receiverOptions = new ReceiverOptions(connectionOptions, autoAcknowledgment);
        
        var receiver = new MessageReceiver(receiverOptions, encoding);
        await receiver.InitializeAsync();
        return receiver;
    }
}