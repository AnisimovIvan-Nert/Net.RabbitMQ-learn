using _Base;
using RabbitMQ.Client.Events;

namespace WorkQueues.Worker;

public class TaskWorker(
    ReceiverOptions options,
    ITaskFactory taskFactory)
    : ReceiverBase<TaskData>(options)
{
    protected override async Task OnReceived(object sender, BasicDeliverEventArgs eventArguments)
    {
        var body = eventArguments.Body.ToArray();
        var taskData = new TaskData(body);
        var task = taskFactory.Create(taskData);
        var result = await task.Execute();
        await RegisterAttemptedData(taskData, result, eventArguments);
    }
}