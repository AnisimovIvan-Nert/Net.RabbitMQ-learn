using PublishSubscribe.Publisher;
using PublishSubscribe.Subscriber;
using RabbitMQ.Client;
using Tests.DockerContainers.RabbitMq;

namespace PublishSubscribe.Tests;

public class PublisherSubscriberIntegrationInMemory : IClassFixture<RabbitMqFixture>
{
    private readonly string _connectionString;

    public PublisherSubscriberIntegrationInMemory(RabbitMqFixture rabbitMqFixture)
    {
        _connectionString = rabbitMqFixture.GetConnectionString();
    }
    
    [Fact]
    public async Task Test()
    {
        const string exchange = nameof(Test);
        const string exchangeType = ExchangeType.Fanout;

        var publisher = await PublisherFactory.CreateAsync(_connectionString, exchange, exchangeType);

        const string messageBeforeSubscriberActive = nameof(messageBeforeSubscriberActive);
        await publisher.SendAsync(messageBeforeSubscriberActive);

        await Task.Delay(10);
        
        var subscriber = await SubscriberFactory.CreateAsync(_connectionString, exchange, exchangeType);
        
        const string messageWhenSubscriberActive = nameof(messageWhenSubscriberActive);
        await publisher.SendAsync(messageWhenSubscriberActive);
        
        await Task.Delay(10);

        await subscriber.DisposeAsync();
        
        const string messageAfterSubscriberActive = nameof(messageAfterSubscriberActive);
        await publisher.SendAsync(messageAfterSubscriberActive);
        
        await Task.Delay(10);

        var handledData = subscriber.PullHandledData().ToArray();

        Assert.Single(handledData);
        Assert.Equal(messageWhenSubscriberActive, handledData.Single());
    }
}