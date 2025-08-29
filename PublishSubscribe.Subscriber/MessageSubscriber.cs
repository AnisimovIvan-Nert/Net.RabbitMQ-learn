using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PublishSubscribe.Subscriber;

public class MessageSubscriber(
    string connectionString,
    string exchange,
    string exchangeType,
    Encoding encoding) 
    : IAsyncDisposable
{
     private readonly List<string> _handledData = [];

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

        await _channel.ExchangeDeclareAsync(exchange, exchangeType);
        
        var queueDeclarationResult = await _channel.QueueDeclareAsync();

        await _channel.QueueBindAsync(queueDeclarationResult.QueueName, exchange, String.Empty);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceived;

        await _channel.BasicConsumeAsync(queueDeclarationResult.QueueName, true, consumer);
    }
    
    public virtual async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
            await _channel.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    }
    
    public IEnumerable<string> PullHandledData()
    {
        var handledData = new List<string>(_handledData);
        _handledData.Clear();
        return handledData;
    }

    private Task OnReceived(object sender, BasicDeliverEventArgs eventArguments)
    {
        var body = eventArguments.Body.ToArray();
        var message = encoding.GetString(body);
        _handledData.Add(message);
        return Task.CompletedTask;
    }
}