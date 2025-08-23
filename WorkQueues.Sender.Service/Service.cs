using Base.Service;
using Base.Service.DelaySource;
using Microsoft.Extensions.Options;
using WorkQueues.Sender.Service.TaskSource;

namespace WorkQueues.Sender.Service;

public class Service : BackgroundService
{
    private readonly ITaskSource _taskSource;
    private readonly IDelaySource _delaySource;
    private readonly RabbitMqConnection _connection;

    private TaskSender? _taskSender;

    public Service(
        ITaskSource taskSource,
        IDelaySource delaySource,
        IOptions<RabbitMqConnection> connectionOptions)
    {
        _taskSource = taskSource;
        _delaySource = delaySource;
        _connection = connectionOptions.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _taskSender = await TaskSenderFactory.CreateAsync(_connection.ConnectionString, _connection.QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await _delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_taskSender == null)
                continue;

            var tasks = await _taskSource.Pull();

            foreach (var task in tasks)
                await _taskSender.SendTaskAsync(task);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_taskSender != null)
            await _taskSender.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}