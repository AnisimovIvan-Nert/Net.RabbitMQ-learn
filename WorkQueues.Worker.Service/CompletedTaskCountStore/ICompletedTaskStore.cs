namespace WorkQueues.Worker.Service.CompletedTaskCountStore;

public interface ICompletedTaskStore
{
    ValueTask AddAsync(TaskData taskData);
}