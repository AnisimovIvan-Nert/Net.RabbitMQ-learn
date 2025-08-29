using System.Text;

namespace PublishSubscribe.Subscriber;

public static class SubscriberFactory
{
    public static async ValueTask<MessageSubscriber> CreateAsync(
        string connectionString, 
        string exchange,
        string exchangeType,
        Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;
        
        var receiver = new MessageSubscriber(connectionString, exchange, exchangeType, encoding);
        await receiver.InitializeAsync();
        return receiver;
    }
}