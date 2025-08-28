using Tests.DockerContainers.RabbitMq;
using WorkQueues.Sender;
using WorkQueues.Tests.Fakes;
using WorkQueues.Worker;

namespace WorkQueues.Tests;

public class SenderWorkerIntegrationInMemory : IClassFixture<RabbitMqFixture>
{
    private const int TasksCount = 10;
    
    private readonly string _connectionString;

    public SenderWorkerIntegrationInMemory(RabbitMqFixture rabbitMqFixture)
    {
        _connectionString = rabbitMqFixture.GetConnectionString();
    }

    [Fact]
    public async Task TaskWorkersHaveEvenDispatching()
    {
        const string queue = nameof(TaskWorkersHaveEvenDispatching);
        
        var executionTime = TimeSpan.FromMilliseconds(10);
        var taskData = DelayedTaskFakeFactory.EncodeTaskData(executionTime);
        
        var taskFactory = new DelayedTaskFakeFactory();
        var firstWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory);
        var secondWorker = await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory);
        
        var taskSender = await TaskSenderFactory.CreateAsync(_connectionString, queue);
        
        
        for (var i = 0; i < TasksCount; i++)
            await taskSender.SendAsync(taskData);

        
        Assert.Empty(firstWorker.PullHandledData());
        Assert.Empty(secondWorker.PullHandledData());

        await Task.Delay(executionTime * TasksCount * 2);

        Assert.Equal(TasksCount / 2, firstWorker.PullHandledData().Count());
        Assert.Equal(TasksCount / 2, secondWorker.PullHandledData().Count());
    }
    
    [Fact]
    public async Task TaskAcknowledgment()
    {
        const string queue = nameof(TaskAcknowledgment);
        
        const int failEveryNTask = 2;
        const int taskCountMultiplier = 2;
        
        const int allTaskCount = TasksCount * failEveryNTask * taskCountMultiplier;
        const int failedTaskCount = allTaskCount / failEveryNTask;
        
        var taskFactory = new FailableTaskFakeFactory();
        var workers = new []
        {
            await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory, false),
            await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory, false),
            await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory, false),
            await TaskWorkerFactory.CreateAsync(_connectionString, queue, taskFactory, false)
        };
        
        var taskSender = await TaskSenderFactory.CreateAsync(_connectionString, queue);


        for (var i = 0; i < allTaskCount; i++)
        {
            var taskData = FailableTaskFakeFactory.EncodeTaskData((i + 1) % failEveryNTask == 0);
            await taskSender.SendAsync(taskData);
        }

        
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        
        var workerHandledTasks = workers.SelectMany(worker => worker.PullHandledData()).Count();
        
        Assert.Equal(allTaskCount - failedTaskCount, workerHandledTasks);
    }
}