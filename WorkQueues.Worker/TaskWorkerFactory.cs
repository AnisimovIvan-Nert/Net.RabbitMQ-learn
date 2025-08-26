namespace WorkQueues.Worker;

public static class TaskWorkerFactory
{
    public static async ValueTask<TaskWorker> CreateAsync(string host, string queue, ITaskFactory taskFactory)
    {
        var receiver = new TaskWorker(host, queue, taskFactory);
        await receiver.InitializeAsync();
        return receiver;
    }
}