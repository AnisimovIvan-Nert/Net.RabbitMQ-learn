using Base.Service;
using Base.Service.DelaySource;
using Microsoft.Extensions.Options;
using WorkQueues.Worker.Service.CompletedTaskCountStore;

namespace WorkQueues.Worker.Service;

public class Service(
    ITaskFactory taskFactory,
    ICompletedTaskStore completedTaskStore,
    IDelaySource delaySource,
    IOptions<RabbitMqConnection> connectionOptions) 
    : BackgroundService
{
    private TaskWorker? _worker;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        var connection = connectionOptions.Value;
        _worker = await TaskWorkerFactory.CreateAsync(connection.ConnectionString, connection.QueueName, taskFactory);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_worker == null)
                continue;

            var completedTasks = _worker.PullCompletedTasks();

            foreach (var completedTask in completedTasks)
                await completedTaskStore.AddAsync(completedTask);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_worker != null)
            await _worker.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}