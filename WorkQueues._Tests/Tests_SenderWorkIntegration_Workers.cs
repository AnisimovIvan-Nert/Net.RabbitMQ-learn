using _Tests.Fakes;
using _Tests.Utilities;
using Base.Service;
using Base.Service.DelaySource;
using Tests.DockerContainers.RabbitMq;
using WorkQueues.Sender.Service.TaskSource;
using WorkQueues.Tests.Fakes;
using WorkQueues.Tests.ServicesApplicationFactories;
using WorkQueues.Worker.Service.CompletedTaskCountStore;

namespace WorkQueues.Tests;

public class SenderReceiverIntegrationWorkers :
    IClassFixture<RabbitMqFixture>,
    IClassFixture<SenderServiceApplicationFactory>,
    IClassFixture<WorkerServiceApplicationFactory>
{
    private const string QueueName = nameof(QueueName);

    private readonly TaskSourceFake _taskSource;
    private readonly CompletedTaskStoreFake _completedTaskStore;
    private readonly DelaySourceFake _senderDelaySourceFake;
    private readonly DelaySourceFake _receiverDelaySourceFake;

    public SenderReceiverIntegrationWorkers(
        RabbitMqFixture rabbitMqFixture,
        SenderServiceApplicationFactory senderServiceApplicationFactory,
        WorkerServiceApplicationFactory workerServiceApplicationFactory)
    {
        var connectionString = rabbitMqFixture.GetConnectionString();

        var rabbitMqConnection = new RabbitMqConnection
        {
            ConnectionString = connectionString,
            QueueName = QueueName
        };

        senderServiceApplicationFactory.AddRabbitMqConnection(rabbitMqConnection);
        workerServiceApplicationFactory.AddRabbitMqConnection(rabbitMqConnection);

        var senderServiceAccess = senderServiceApplicationFactory.GetServiceAccess();
        _taskSource = senderServiceAccess.GetService<ITaskSource, TaskSourceFake>();
        _senderDelaySourceFake = senderServiceAccess.GetService<IDelaySource, DelaySourceFake>();

        var receiverServiceAccess = workerServiceApplicationFactory.GetServiceAccess();
        _completedTaskStore = receiverServiceAccess.GetService<ICompletedTaskStore, CompletedTaskStoreFake>();
        _receiverDelaySourceFake = senderServiceAccess.GetService<IDelaySource, DelaySourceFake>();
    }

    [Fact]
    public async Task ReceiverReceiveMessageFromSender()
    {
        var executionTime = TimeSpan.FromMilliseconds(10);
        var taskData = TaskFactoryFake.EncodeTaskData(executionTime);

        for (var i = 0; i < 4; i++)
            _taskSource.Push(taskData);

        Assert.Empty(_completedTaskStore.Store);

        await Task.Delay(_senderDelaySourceFake.Delay + _receiverDelaySourceFake.Delay);

        Assert.Equal(4, _completedTaskStore.Store.Count);
    }
}