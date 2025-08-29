using Base;

namespace WorkQueues.Worker;

public static class TaskWorkerFactory
{
    public static async ValueTask<TaskWorker> CreateAsync(
        string connectionString, 
        string queue, 
        ITaskFactory taskFactory,
        bool autoAcknowledgment = true)
    {
        var receiverOptions = new ReceiverOptions(connectionString, queue, autoAcknowledgment);
        
        var receiver = new TaskWorker(receiverOptions, taskFactory);
        await receiver.InitializeAsync();
        return receiver;
    }
}