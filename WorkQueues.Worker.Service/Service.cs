using Base;
using Base.Receiver;
using Base.Service;
using Base.Service.Configurations;
using Base.Service.Services;
using Microsoft.Extensions.Options;

namespace WorkQueues.Worker.Service;

public class Service(
    ITaskFactory taskFactory,
    IDataStore<TaskData> messageStore,
    IDelaySource delaySource,
    IOptions<RabbitMqConnection> connectionOptions)
    : ReceiverServiceBase<TaskData>(messageStore, delaySource)
{
    protected override async ValueTask<ReceiverBase<TaskData>> CreateReceiverAsync()
    {
        var connection = connectionOptions.Value;
        return await TaskWorkerFactory.CreateAsync(connection.ConnectionString, connection.QueueName, taskFactory);
    }
}