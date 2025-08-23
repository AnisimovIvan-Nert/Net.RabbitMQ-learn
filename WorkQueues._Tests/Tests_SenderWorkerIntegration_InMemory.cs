using Tests.DockerContainers.RabbitMq;
using WorkQueues.Sender;
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
        var task = new TaskData(TimeSpan.FromMilliseconds(10));

        var taskSender = await TaskSenderFactory.CreateAsync(_connectionString, queue);

        var firstWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue);
        var secondWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue);

        for (var i = 0; i < 4; i++)
            await taskSender.SendTaskAsync(task);

        Assert.Empty(firstWorker.PullCompletedTasks());
        Assert.Empty(secondWorker.PullCompletedTasks());

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Assert.Equal(2, firstWorker.PullCompletedTasks().Count());
        Assert.Equal(2, secondWorker.PullCompletedTasks().Count());
    }
}