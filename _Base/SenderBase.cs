using RabbitMQ.Client;

namespace _Base;

public abstract class SenderBase<TData>(
    string connectionString, 
    string queue)
    : IAsyncDisposable
{
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
    }

    public async ValueTask SendAsync(TData message)
    {
        if (_channel == null)
            throw new InvalidOperationException();

        var body = EncodeData(message);
        await _channel.BasicPublishAsync(string.Empty, queue, body);
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