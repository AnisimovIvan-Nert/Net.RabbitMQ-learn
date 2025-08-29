using Base;
using Base.Receiver;

namespace WorkQueues.Worker;

public static class TaskWorkerFactory
{
    public static async ValueTask<TaskWorker> CreateAsync(
        string connectionString, 
        string queue, 
        ITaskFactory taskFactory,
        bool durable = false,
        bool autoAcknowledgment = true)
    {
        var connectionOptions = new ConnectionOptions(connectionString, queue, durable);
        var receiverOptions = new ReceiverOptions(connectionOptions, autoAcknowledgment);
        
        var receiver = new TaskWorker(receiverOptions, taskFactory);
        await receiver.InitializeAsync();
        return receiver;
    }
}