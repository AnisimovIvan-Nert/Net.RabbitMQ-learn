using RabbitMQ.Client;

namespace WorkQueues.Sender;

public class TaskSender : IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly string _queue;

    private IConnection? _connection;
    private IChannel? _channel;

    internal TaskSender(string connectionString, string queue)
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

    public async ValueTask SendTaskAsync(TaskData taskData)
    {
        if (_channel == null)
            throw new InvalidOperationException();

        var body = taskData.GetData();
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