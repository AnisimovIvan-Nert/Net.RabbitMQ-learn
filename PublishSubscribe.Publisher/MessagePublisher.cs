using System.Text;
using RabbitMQ.Client;

namespace PublishSubscribe.Publisher;

public class MessagePublisher(
    string connectionString, 
    string exchange, 
    string exchangeType, 
    Encoding encoding) 
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
        
        await _channel.ExchangeDeclareAsync(exchange, exchangeType);
    }
    
    public async ValueTask SendAsync(string message, string routingKey = "")
    {
        if (_channel == null)
            throw new InvalidOperationException();
        
        var body = EncodeData(message);
        await _channel.BasicPublishAsync(exchange, routingKey, body);
    }
    
    private byte[] EncodeData(string data)
    {
        return encoding.GetBytes(data);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
            await _channel.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}