using Base;
using Base.Sender;

namespace WorkQueues.Sender;

public static class TaskSenderFactory
{
    public static async ValueTask<TaskSender> CreateAsync(
        string connectionString, 
        string queue,
        bool durable = false)
    {
        var connectionOptions = new ConnectionOptions(connectionString, queue, durable);
        var senderOptions = new SenderOptions(connectionOptions);
        
        var sender = new TaskSender(senderOptions);
        await sender.InitializeAsync();
        return sender;
    }
}