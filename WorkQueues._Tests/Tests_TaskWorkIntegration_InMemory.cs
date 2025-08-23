using Tests.DockerContainers.RabbitMq;
using WorkQueues.Task;
using WorkQueues.Worker;

namespace WorkQueues._Tests;

public class TaskWorkIntegrationInMemory : IClassFixture<RabbitMqFixture>
{
    private readonly string _connectionString;

    public TaskWorkIntegrationInMemory(RabbitMqFixture rabbitMqFixture)
    {
        _connectionString = rabbitMqFixture.GetConnectionString();
    }

    [Fact]
    public async System.Threading.Tasks.Task TaskWorkersHaveEvenDispatching()
    {
        const string queue = nameof(queue);

        var taskSender = await TaskSenderFactory.CreateAsync(_connectionString, queue);
        
        var firstWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue);
        var secondWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue);

        for (var i = 0; i < 4; i++)
            await taskSender.SendTaskAsync(TimeSpan.FromMilliseconds(10));
        
        Assert.Equal(0, firstWorker.CompletedTaskCount);
        Assert.Equal(0, secondWorker.CompletedTaskCount);

        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(100));

        Assert.Equal(2, firstWorker.CompletedTaskCount);
        Assert.Equal(2, secondWorker.CompletedTaskCount);
    }
}