using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkQueues.Worker;

public class TaskWorker : IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly string _queue;

    private IConnection? _connection;
    private IChannel? _channel;

    private int _completedTaskCount;

    public int CompletedTaskCount => _completedTaskCount;

    internal TaskWorker(string connectionString, string queue)
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
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var taskTime = BitConverter.ToInt32(body);
            await Task.Delay(TimeSpan.FromMilliseconds(taskTime));
            Interlocked.Increment(ref _completedTaskCount);
        };

        await _channel.BasicConsumeAsync(_queue, true, consumer);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();

        if (_channel != null)
            await _channel.DisposeAsync();
    }
}