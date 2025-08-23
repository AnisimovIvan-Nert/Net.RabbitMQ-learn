using HelloWorld.Receive;
using Tests.DockerContainers.RabbitMq;

namespace HelloWorld.Send.Tests;

public class SenderReceiverIntegrationInMemory : IClassFixture<RabbitMqFixture>
{
    private readonly string _connectionString;

    public SenderReceiverIntegrationInMemory(RabbitMqFixture rabbitMqFixture)
    {
        _connectionString = rabbitMqFixture.GetConnectionString();
    }

    [Fact]
    public async Task ReceiverReceiveMessageFromSender()
    {
        const string message = nameof(message);
        const string queue = nameof(queue);

        var sender = await SenderFactory.CreateAsync(_connectionString, queue);
        var receiver = await ReceiverFactory.CreateAsync(_connectionString, queue);

        await sender.SendMessageAsync(message);

        await Task.Delay(100);

        var receivedMessages = receiver.PullMessages().ToArray();

        Assert.Single(receivedMessages);
        Assert.Equal(message, receivedMessages.Single());
    }
}