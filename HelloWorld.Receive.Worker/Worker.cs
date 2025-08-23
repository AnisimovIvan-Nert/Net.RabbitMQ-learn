using HelloWorld.Receive.Worker.MessageStore;
using HelloWorld.Worker;
using HelloWorld.Worker.DelaySource;
using Microsoft.Extensions.Options;

namespace HelloWorld.Receive.Worker;

public class Worker : BackgroundService
{
    private readonly IMessageStore _messageStore;
    private readonly IDelaySource _delaySource;
    private readonly RabbitMqConnection _connection;

    private Receiver? _receiver;

    public Worker(
        IMessageStore messageStore,
        IDelaySource delaySource,
        IOptions<RabbitMqConnection> connectionOptions)
    {
        _messageStore = messageStore;
        _delaySource = delaySource;
        _connection = connectionOptions.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _receiver = await ReceiverFactory.CreateAsync(_connection.ConnectionString, _connection.QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await _delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_receiver == null)
                continue;

            var messages = _receiver.PullMessages();

            foreach (var message in messages)
                await _messageStore.AddAsync(message);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_receiver != null)
            await _receiver.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}