using _Base;
using Base.Service;
using Base.Service.Configurations;
using Base.Service.Services;
using Microsoft.Extensions.Options;

namespace HelloWorld.Receive.Service;

public class Service(
    IDataStore<string> messageStore,
    IDelaySource delaySource,
    IOptions<RabbitMqConnection> connectionOptions,
    IOptions<RabbitMqReceiverAcknowledgment>? acknowledgmentOptions = null)
    : ReceiverServiceBase<string>(messageStore, delaySource)
{
    protected override async ValueTask<ReceiverBase<string>> CreateReceiverAsync()
    {
        var connection = connectionOptions.Value;

        var connectionString = connection.ConnectionString;
        var queue = connection.QueueName;
        
        return acknowledgmentOptions == null 
            ? await ReceiverFactory.CreateAsync(connectionString, queue) 
            : await ReceiverFactory.CreateAsync(connectionString, queue, acknowledgmentOptions.Value.Value);
    }
}