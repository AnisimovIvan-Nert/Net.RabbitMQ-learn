using System.Text;
using RabbitMQ.Client;

namespace HelloWorld.Send;

public class Sender : IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly string _queue;

    private IConnection? _connection;
    private IChannel? _channel;

    internal Sender(string connectionString, string queue)
    {
        _connectionString = connectionString;
        _queue = queue;
    }

    internal async ValueTask InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_connectionString)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(_queue, false, false, false);
    }

    public async ValueTask SendMessageAsync(string message)
    {
        if (_channel == null)
            throw new InvalidOperationException();

        var body = Encoding.Unicode.GetBytes(message);
        await _channel.BasicPublishAsync(string.Empty, _queue, body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
        
        if (_channel != null)
            await _channel.DisposeAsync();
    }
}