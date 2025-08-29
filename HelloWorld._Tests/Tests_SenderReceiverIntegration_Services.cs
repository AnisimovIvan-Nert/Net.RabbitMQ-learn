using Base.Service.Configurations;
using HelloWorld.Send.Tests.ServiceApplications;
using Tests.DockerContainers.RabbitMq;
using Tests.Fixtures;

namespace HelloWorld.Send.Tests;

public class SenderReceiverIntegrationServices :
    IClassFixture<RabbitMqFixture>,
    IClassFixture<ApplicationFactoryFixture<SenderServiceApplicationFactory>>,
    IClassFixture<ApplicationFactoryFixture<ReceiverServiceApplicationFactory>>
{
    private const string QueueName = nameof(SenderReceiverIntegrationServices);

    private readonly SenderServiceApplicationAccess _senderAccess;
    private readonly ReceiverServiceApplicationAccess _receiverAccess;

    public SenderReceiverIntegrationServices(
        RabbitMqFixture rabbitMqFixture,
        ApplicationFactoryFixture<SenderServiceApplicationFactory> senderApplicationFactoryFixture,
        ApplicationFactoryFixture<ReceiverServiceApplicationFactory> receiverApplicationFactoryFixture)
    {
        var connectionString = rabbitMqFixture.GetConnectionString();

        var rabbitMqConnection = new RabbitMqConnection
        {
            ConnectionString = connectionString,
            QueueName = QueueName
        };

        var senderServiceApplicationFactory = senderApplicationFactoryFixture.CreateFactory();
        senderServiceApplicationFactory.Initialize(rabbitMqConnection);
        _senderAccess = senderServiceApplicationFactory.GetApplicationAccess();

        var receiverServiceApplicationFactory = receiverApplicationFactoryFixture.CreateFactory();
        receiverServiceApplicationFactory.Initialize(rabbitMqConnection);
        _receiverAccess = receiverServiceApplicationFactory.GetApplicationAccess();
    }

    [Fact]
    public async Task ReceiverReceiveMessageFromSender()
    {
        const string message = nameof(message);

        
        _senderAccess.MessageSource.Push(message);
        await Task.Delay(_senderAccess.DelaySource.Delay + _receiverAccess.DelaySource.Delay);

        
        var receivedMessages = _receiverAccess.MessageStore.Store;
        Assert.Single(receivedMessages);
        Assert.Equal(message, receivedMessages.Single());
    }
}