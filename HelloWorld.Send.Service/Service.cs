using Base;
using Base.Sender;
using Base.Service;
using Base.Service.Configurations;
using Base.Service.Services;
using Microsoft.Extensions.Options;

namespace HelloWorld.Send.Service;

public class Service(
    IDataSource<string> messageSource,
    IDelaySource delaySource,
    IOptions<RabbitMqConnection> connectionOptions)
    : SenderServiceBase<string>(messageSource, delaySource)
{
    protected override async ValueTask<SenderBase<string>> CreateSenderAsync()
    {
        var connection = connectionOptions.Value;
        return await SenderFactory.CreateAsync(connection.ConnectionString, connection.QueueName);
    }
}