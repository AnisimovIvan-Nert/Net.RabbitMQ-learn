namespace WorkQueues.Worker;

public static class TaskWorkerFactory
{
    public static async ValueTask<TaskWorker> CreateAsync(string host, string queue)
    {
        var receiver = new TaskWorker(host, queue);
        await receiver.InitializeAsync();
        return receiver;
    }
}