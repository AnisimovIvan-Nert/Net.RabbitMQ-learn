using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _Base;

public record ReceiverOptions(
    string ConnectionString,
    string Queue,
    bool AutoAcknowledgement = true);

public abstract class ReceiverBase<TData>(ReceiverOptions options)
    : IAsyncDisposable
{
    private readonly List<TData> _handledData = [];
    private readonly List<TData> _attemptedData = [];

    private IConnection? _connection;
    private IChannel? _channel;

    private IChannel Channel => _channel ?? throw new InvalidOperationException();
    
    public async ValueTask InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(options.ConnectionString)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(options.Queue, false, false, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceived;

        await _channel.BasicConsumeAsync(options.Queue, options.AutoAcknowledgement, consumer);
    }
    
    public virtual async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
            await _channel.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    }
    
    public IEnumerable<TData> PullHandledData()
    {
        var handledData = new List<TData>(_handledData);
        _handledData.Clear();
        return handledData;
    }
    
    public IEnumerable<TData> PullAttemptedData()
    {
        var attemptedData = new List<TData>(_attemptedData);
        _attemptedData.Clear();
        return attemptedData;
    }

    protected abstract Task OnReceived(object sender, BasicDeliverEventArgs eventArguments);

    protected async ValueTask RegisterAttemptedData(TData data, bool result, BasicDeliverEventArgs eventArguments)
    {
        _attemptedData.Add(data);
        
        if (result)
            _handledData.Add(data);

        await PerformAcknowledgement(eventArguments, result);
    }
    
    private ValueTask PerformAcknowledgement(
        BasicDeliverEventArgs eventArguments,
        bool acknowledgment)
    {
        if (options.AutoAcknowledgement)
            return ValueTask.CompletedTask;

        return acknowledgment
            ? Channel.BasicAckAsync(eventArguments.DeliveryTag, false)
            : Channel.BasicNackAsync(eventArguments.DeliveryTag, false, true);
    }
}