namespace WorkQueues.Worker;

public static class TaskWorkerFactory
{
    public static async ValueTask<TaskWorker> CreateAsync(string connectionString, string queue, ITaskFactory taskFactory)
    {
        var receiver = new TaskWorker(connectionString, queue, taskFactory);
        await receiver.InitializeAsync();
        return receiver;
    }
}