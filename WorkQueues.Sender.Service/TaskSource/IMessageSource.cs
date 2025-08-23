namespace WorkQueues.Sender.Service.TaskSource;

public interface ITaskSource
{
    ValueTask<IEnumerable<TaskData>> Pull();
}