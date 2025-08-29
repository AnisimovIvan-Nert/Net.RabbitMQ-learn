using System.Text;

namespace PublishSubscribe.Publisher;

public static class PublisherFactory
{
    public static async ValueTask<MessagePublisher> CreateAsync(
        string connectionString,
        string exchange,
        string exchangeType,
        Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var publisher = new MessagePublisher(connectionString, exchange, exchangeType, encoding);
        await publisher.InitializeAsync();
        return publisher;
    }
}