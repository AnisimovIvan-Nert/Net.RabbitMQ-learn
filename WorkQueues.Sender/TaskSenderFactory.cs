namespace WorkQueues.Sender;

public static class TaskSenderFactory
{
    public static async ValueTask<TaskSender> CreateAsync(string connectionString, string queue)
    {
        var sender = new TaskSender(connectionString, queue);
        await sender.InitializeAsync();
        return sender;
    }
}