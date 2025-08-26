namespace WorkQueues;

public interface ITaskFactory
{
    public ITask Create(TaskData taskData);
}