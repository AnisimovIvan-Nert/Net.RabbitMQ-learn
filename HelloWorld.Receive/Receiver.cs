using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HelloWorld.Receive;

public class Receiver : IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly string _queue;
    private readonly List<string> _receivedMessages = [];

    private IConnection? _connection;
    private IChannel? _channel;

    internal Receiver(string connectionString, string queue)
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
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.Unicode.GetString(body);
            _receivedMessages.Add(message);
            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(_queue, true, consumer);
    }
    
    public IEnumerable<string> PullMessages()
    {
        return _receivedMessages;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
        
        if (_channel != null)
            await _channel.DisposeAsync();
    }
}