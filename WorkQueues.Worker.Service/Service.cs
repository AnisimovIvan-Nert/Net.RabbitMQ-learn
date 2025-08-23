using Base.Service;
using Base.Service.DelaySource;
using Microsoft.Extensions.Options;
using WorkQueues.Worker.Service.CompletedTaskCountStore;

namespace WorkQueues.Worker.Service;

public class Service : BackgroundService
{
    private readonly ICompletedTaskStore _completedTaskStore;
    private readonly IDelaySource _delaySource;
    private readonly RabbitMqConnection _connection;

    private TaskWorker? _worker;

    public Service(
        ICompletedTaskStore completedTaskStore,
        IDelaySource delaySource,
        IOptions<RabbitMqConnection> connectionOptions)
    {
        _completedTaskStore = completedTaskStore;
        _delaySource = delaySource;
        _connection = connectionOptions.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _worker = await TaskWorkerFactory.CreateAsync(_connection.ConnectionString, _connection.QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await _delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_worker == null)
                continue;

            var completedTasks = _worker.PullCompletedTasks();

            foreach (var completedTask in completedTasks)
                await _completedTaskStore.AddAsync(completedTask);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_worker != null)
            await _worker.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}