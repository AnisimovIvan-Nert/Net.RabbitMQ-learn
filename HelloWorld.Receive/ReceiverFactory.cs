using System.Text;
using Base;

namespace HelloWorld.Receive;

public static class ReceiverFactory
{
    public static async ValueTask<MessageReceiver> CreateAsync(
        string connectionString, 
        string queue, 
        bool autoAcknowledgment = true, 
        Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var receiverOptions = new ReceiverOptions(connectionString, queue, autoAcknowledgment);
        
        var receiver = new MessageReceiver(receiverOptions, encoding);
        await receiver.InitializeAsync();
        return receiver;
    }
}