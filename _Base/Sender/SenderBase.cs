using RabbitMQ.Client;

namespace Base.Sender;

public abstract class SenderBase<TData>(SenderOptions options)
    : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async ValueTask InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(options.ConnectionOptions.ConnectionString)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        
        await _channel.QueueDeclareAsync(options.ConnectionOptions.Queue, false, false, false);
    }

    public async ValueTask SendAsync(TData message, SendProperties properties = default)
    {
        if (_channel == null)
            throw new InvalidOperationException();

        var exchange = string.Empty;
        var routingKey = options.ConnectionOptions.Queue;
        const bool mandatory = false;
        var publishProperties = new BasicProperties
        {
            Persistent = properties.Persistent
        };
        var body = EncodeData(message);
        await _channel.BasicPublishAsync(exchange, routingKey, mandatory, publishProperties, body);
    }

    protected abstract byte[] EncodeData(TData data);

    public virtual async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
            await _channel.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}