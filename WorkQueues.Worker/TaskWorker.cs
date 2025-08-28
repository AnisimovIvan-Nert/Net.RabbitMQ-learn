using _Base;
using RabbitMQ.Client.Events;

namespace WorkQueues.Worker;

public class TaskWorker(
    string connectionString, 
    string queue, 
    ITaskFactory taskFactory) 
    : ReceiverBase<TaskData>(connectionString, queue)
{
    protected override async Task OnReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (sender is not AsyncEventingBasicConsumer eventConsumer)
            throw new InvalidOperationException();
            
        var body = eventArgs.Body.ToArray();
        var taskData = new TaskData(body);
        var task = taskFactory.Create(taskData);
        await task.Execute();
        
        RegisterHandledData(taskData);
    }
}