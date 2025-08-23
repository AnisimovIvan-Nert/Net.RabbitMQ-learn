using Base.Service;
using Base.Service.DelaySource;
using HelloWorld.Receive.Service.MessageStore;
using HelloWorld.Sen.Service.MessageSource;
using HelloWorld.Send.Tests.Fakes;
using HelloWorld.Send.Tests.ServicesApplicationFactories;
using HelloWorld.Send.Tests.Utilities;
using Tests.DockerContainers.RabbitMq;

namespace HelloWorld.Send.Tests;

public class SenderReceiverIntegrationWorkers :
    IClassFixture<RabbitMqFixture>,
    IClassFixture<SenderServiceApplicationFactory>,
    IClassFixture<ReceiverServiceApplicationFactory>
{
    private const string QueueName = nameof(QueueName);

    private readonly MessageSourceFake _messageSource;
    private readonly MessageStoreFake _messageStore;
    private readonly DelaySourceFake _senderDelaySourceFake;
    private readonly DelaySourceFake _receiverDelaySourceFake;

    public SenderReceiverIntegrationWorkers(
        RabbitMqFixture rabbitMqFixture,
        SenderServiceApplicationFactory senderServiceApplicationFactory,
        ReceiverServiceApplicationFactory receiverServiceApplicationFactory)
    {
        var connectionString = rabbitMqFixture.GetConnectionString();

        var rabbitMqConnection = new RabbitMqConnection
        {
            ConnectionString = connectionString,
            QueueName = QueueName
        };

        senderServiceApplicationFactory.AddRabbitMqConnection(rabbitMqConnection);
        receiverServiceApplicationFactory.AddRabbitMqConnection(rabbitMqConnection);

        var senderServiceAccess = senderServiceApplicationFactory.GetServiceAccess();
        _messageSource = senderServiceAccess.GetService<IMessageSource, MessageSourceFake>();
        _senderDelaySourceFake = senderServiceAccess.GetService<IDelaySource, DelaySourceFake>();

        var receiverServiceAccess = receiverServiceApplicationFactory.GetServiceAccess();
        _messageStore = receiverServiceAccess.GetService<IMessageStore, MessageStoreFake>();
        _receiverDelaySourceFake = senderServiceAccess.GetService<IDelaySource, DelaySourceFake>();
    }

    [Fact]
    public async Task ReceiverReceiveMessageFromSender()
    {
        const string message = nameof(message);

        _messageSource.Push(message);

        await Task.Delay(_senderDelaySourceFake.Delay + _receiverDelaySourceFake.Delay);

        var receivedMessages = _messageStore.Store;

        Assert.Single(receivedMessages);
        Assert.Equal(message, receivedMessages.Single());
    }
}