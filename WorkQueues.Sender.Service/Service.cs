using Base;
using Base.Sender;
using Base.Service;
using Base.Service.Configurations;
using Base.Service.Services;
using Microsoft.Extensions.Options;

namespace WorkQueues.Sender.Service;

public class Service(
    IDataSource<TaskData> messageSource,
    IDelaySource delaySource,
    IOptions<RabbitMqConnection> connectionOptions)
    : SenderServiceBase<TaskData>(messageSource, delaySource)
{
    protected override async ValueTask<SenderBase<TaskData>> CreateSenderAsync()
    {
        var connection = connectionOptions.Value;
        return await TaskSenderFactory.CreateAsync(connection.ConnectionString, connection.QueueName);
    }
}