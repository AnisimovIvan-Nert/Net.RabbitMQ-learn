using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _Base;

public abstract class ReceiverBase<TData>(
    string connectionString, 
    string queue)
    : IAsyncDisposable
{
    private readonly List<TData> _handledData = [];

    private IConnection? _connection;
    private IChannel? _channel;

    public async ValueTask InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue, false, false, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceived;

        await _channel.BasicConsumeAsync(queue, true, consumer);
    }

    protected abstract Task OnReceived(object sender, BasicDeliverEventArgs eventArgs);

    protected void RegisterHandledData(TData data)
    {
        _handledData.Add(data);
    }
    
    public IEnumerable<TData> PullHandledData()
    {
        var handledData = new List<TData>(_handledData);
        _handledData.Clear();
        return handledData;
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
            await _channel.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}