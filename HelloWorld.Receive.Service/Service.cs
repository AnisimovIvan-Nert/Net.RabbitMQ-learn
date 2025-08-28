using _Base;
using Base.Service;
using Base.Service.Services;
using Microsoft.Extensions.Options;

namespace HelloWorld.Receive.Service;

public class Service(
    IDataStore<string> messageStore,
    IDelaySource delaySource,
    IOptions<RabbitMqConnection> connectionOptions)
    : ReceiverServiceBase<string>(messageStore, delaySource)
{
    protected override async ValueTask<ReceiverBase<string>> CreateReceiverAsync()
    {
        var connection = connectionOptions.Value;
        return await ReceiverFactory.CreateAsync(connection.ConnectionString, connection.QueueName);
    }
}