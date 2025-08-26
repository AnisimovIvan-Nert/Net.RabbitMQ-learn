using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkQueues.Worker;

public class TaskWorker(
    string connectionString, 
    string queue, 
    ITaskFactory taskFactory) : IAsyncDisposable
{
    private readonly List<TaskData> _completedTasks = [];

    private IConnection? _connection;
    private IChannel? _channel;

    internal async ValueTask InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue, false, false, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            if (sender is not AsyncEventingBasicConsumer eventConsumer)
                throw new InvalidOperationException();
            
            var body = eventArgs.Body.ToArray();
            var taskData = new TaskData(body);
            var task = taskFactory.Create(taskData);
            await task.Execute();
            
            _completedTasks.Add(taskData);
        };

        await _channel.BasicConsumeAsync(queue, true, consumer);
    }

    public IEnumerable<TaskData> PullCompletedTasks()
    {
        var completedTasks = new List<TaskData>(_completedTasks);
        _completedTasks.Clear();
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