using Tests.DockerContainers.RabbitMq;
using WorkQueues.Sender;
using WorkQueues.Tests.Fakes;
using WorkQueues.Worker;

namespace WorkQueues.Tests;

public class SenderWorkerIntegrationInMemory : IClassFixture<RabbitMqFixture>
{
    private readonly string _connectionString;

    public SenderWorkerIntegrationInMemory(RabbitMqFixture rabbitMqFixture)
    {
        _connectionString = rabbitMqFixture.GetConnectionString();
    }

    [Fact]
    public async Task TaskWorkersHaveEvenDispatching()
    {
        const string queue = nameof(queue);
        
        var executionTime = TimeSpan.FromMilliseconds(10);
        var taskData = TaskFactoryFake.EncodeTaskData(executionTime);
        
        var taskFactory = new TaskFactoryFake();
        var firstWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory);
        var secondWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory);
        
        var taskSender = await TaskSenderFactory.CreateAsync(_connectionString, queue);
        
        
        for (var i = 0; i < 4; i++)
            await taskSender.SendTaskAsync(taskData);

        
        Assert.Empty(firstWorker.PullCompletedTasks());
        Assert.Empty(secondWorker.PullCompletedTasks());

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Assert.Equal(2, firstWorker.PullCompletedTasks().Count());
        Assert.Equal(2, secondWorker.PullCompletedTasks().Count());
    }
}