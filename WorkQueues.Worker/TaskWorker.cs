using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkQueues.Worker;

public class TaskWorker : IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly string _queue;
    private readonly List<TaskData> _completedTasks = [];

    private IConnection? _connection;
    private IChannel? _channel;

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
            var taskData = new TaskData(body);
            var executionTime = taskData.GetExecutionTime();
            await Task.Delay(executionTime);
            _completedTasks.Add(taskData);
        };

        await _channel.BasicConsumeAsync(_queue, true, consumer);
    }

    public IEnumerable<TaskData> PullCompletedTasks()
    {
        var completedTasks = new List<TaskData>(_completedTasks);
        _completedTasks.Clear();
        ;
        return completedTasks;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
            await _channel.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}