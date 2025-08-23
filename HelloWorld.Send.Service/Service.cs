using HelloWorld.Sen.Service.MessageSource;
using HelloWorld.Send;
using HelloWorld.Service;
using HelloWorld.Service.DelaySource;
using Microsoft.Extensions.Options;

namespace HelloWorld.Sen.Service;

public class Service : BackgroundService
{
    private readonly IMessageSource _messageSource;
    private readonly IDelaySource _delaySource;
    private readonly RabbitMqConnection _connection;

    private Sender? _sender;

    public Service(
        IMessageSource messageSource,
        IDelaySource delaySource,
        IOptions<RabbitMqConnection> connectionOptions)
    {
        _messageSource = messageSource;
        _delaySource = delaySource;
        _connection = connectionOptions.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _sender = await SenderFactory.CreateAsync(_connection.ConnectionString, _connection.QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await _delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_sender == null)
                continue;

            var messages = await _messageSource.Pull();

            foreach (var message in messages)
                await _sender.SendMessageAsync(message);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_sender != null)
            await _sender.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}