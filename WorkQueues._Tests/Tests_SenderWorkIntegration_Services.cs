using Base.Service.Configurations;
using Tests.DockerContainers.RabbitMq;
using Tests.Fixtures;
using WorkQueues.Tests.Fakes;
using WorkQueues.Tests.ServicesApplicationFactories;

namespace WorkQueues.Tests;

public class SenderReceiverIntegrationServices :
    IClassFixture<RabbitMqFixture>,
    IClassFixture<ApplicationFactoryFixture<SenderServiceApplicationFactory>>,
    IClassFixture<ApplicationFactoryFixture<WorkerServiceApplicationFactory>>
{
    private const string QueueName = nameof(SenderReceiverIntegrationServices);

    private const int TaskCount = 12;
    private const int WorkerCount = 5;

    private readonly SenderServiceApplicationAccess _senderAccess;
    private readonly WorkerServiceApplicationAccess[] _workerAccesses;

    public SenderReceiverIntegrationServices(
        RabbitMqFixture rabbitMqFixture,
        ApplicationFactoryFixture<SenderServiceApplicationFactory> senderServiceApplicationFactoryFixture,
        ApplicationFactoryFixture<WorkerServiceApplicationFactory> workerServiceApplicationFactoryFixture)
    {
        var connectionString = rabbitMqFixture.GetConnectionString();

        var rabbitMqConnection = new RabbitMqConnection
        {
            ConnectionString = connectionString,
            QueueName = QueueName
        };

        var senderServiceApplicationFactory = senderServiceApplicationFactoryFixture.CreateFactory();
        senderServiceApplicationFactory.Initialize(rabbitMqConnection);
        _senderAccess = senderServiceApplicationFactory.GetApplicationAccess();

        _workerAccesses = new WorkerServiceApplicationAccess[WorkerCount];
        for (var i = 0; i < WorkerCount; i++)
        {
            var workerServiceApplicationFactory = workerServiceApplicationFactoryFixture.CreateFactory();
            workerServiceApplicationFactory.Initialize(rabbitMqConnection);
            _workerAccesses[i] = workerServiceApplicationFactory.GetApplicationAccess();
        }
    }

    [Fact]
    public async Task TaskWorkersHaveEvenDispatching()
    {
        var executionTime = TimeSpan.FromMilliseconds(10);
        var taskData = DelayedTaskFakeFactory.EncodeTaskData(executionTime);

        
        for (var i = 0; i < TaskCount; i++)
            _senderAccess.TaskSource.Push(taskData);

        
        Assert.All(_workerAccesses, access => Assert.Empty(access.TaskStore.Store));

        var taskPerWorkerRoundUp = (int)Math.Ceiling((decimal)TaskCount / WorkerCount);
        var senderDelay = _senderAccess.DelaySource.Delay;
        var workerDelay = _workerAccesses.First().DelaySource.Delay;
        var approximatelyTasksExecutionTime = executionTime * taskPerWorkerRoundUp;
        await Task.Delay(senderDelay + workerDelay + approximatelyTasksExecutionTime);
        
        const int eventTasksPerWorker = TaskCount / WorkerCount;
        const int restTask = TaskCount % WorkerCount;
        
        var expectedTasksDispatching = Enumerable.Repeat(eventTasksPerWorker, WorkerCount).ToArray();
        for (var i = 0; i < restTask; i++)
            expectedTasksDispatching[i]++;

        var actualTasksDispatching = _workerAccesses.Select(access => access.TaskStore.Store.Count)
            .OrderDescending()
            .ToArray();
        
        Assert.Equivalent(expectedTasksDispatching, actualTasksDispatching);
    }
}