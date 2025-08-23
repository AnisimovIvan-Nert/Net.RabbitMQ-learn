using HelloWorld.Receive.Worker.MessageStore;
using HelloWorld.Send.Tests.Fakes;
using HelloWorld.Send.Tests.Utilities;
using HelloWorld.Send.Tests.WorkersApplicationFactories;
using HelloWorld.Send.Worker.MessageSource;
using HelloWorld.Worker;
using HelloWorld.Worker.DelaySource;
using Tests.DockerContainers.RabbitMq;

namespace HelloWorld.Send.Tests;

public class SenderReceiverIntegrationWorkers :
    IClassFixture<RabbitMqFixture>,
    IClassFixture<SenderWorkerApplicationFactory>,
    IClassFixture<ReceiverWorkerApplicationFactory>
{
    private const string QueueName = nameof(QueueName);

    private readonly MessageSourceFake _messageSource;
    private readonly MessageStoreFake _messageStore;
    private readonly DelaySourceFake _senderDelaySourceFake;
    private readonly DelaySourceFake _receiverDelaySourceFake;

    public SenderReceiverIntegrationWorkers(
        RabbitMqFixture rabbitMqFixture,
        SenderWorkerApplicationFactory senderWorkerApplicationFactory,
        ReceiverWorkerApplicationFactory receiverWorkerApplicationFactory)
    {
        var connectionString = rabbitMqFixture.GetConnectionString();

        var rabbitMqConnection = new RabbitMqConnection
        {
            ConnectionString = connectionString,
            QueueName = QueueName
        };

        senderWorkerApplicationFactory.AddRabbitMqConnection(rabbitMqConnection);
        receiverWorkerApplicationFactory.AddRabbitMqConnection(rabbitMqConnection);

        var senderServiceAccess = senderWorkerApplicationFactory.GetServiceAccess();
        _messageSource = senderServiceAccess.GetService<IMessageSource, MessageSourceFake>();
        _senderDelaySourceFake = senderServiceAccess.GetService<IDelaySource, DelaySourceFake>();

        var receiverServiceAccess = receiverWorkerApplicationFactory.GetServiceAccess();
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