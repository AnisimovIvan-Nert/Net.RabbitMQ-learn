namespace WorkQueues.Tests.Fakes;

public class DelayedTaskFake(TimeSpan executionTime) : ITask
{
    public async ValueTask<bool> Execute()
    {
        await Task.Delay(executionTime);
        return true;
    }
}

public class DelayedTaskFakeFactory : ITaskFactory
{
    public ITask Create(TaskData taskData)
    {
        var executionTime = DecodeTaskData(taskData);
        return new DelayedTaskFake(executionTime);
    }

    public static TaskData EncodeTaskData(TimeSpan executionTime)
    {
        var milliseconds = executionTime.Milliseconds;
        var encodedData = BitConverter.GetBytes(milliseconds);
        return new TaskData(encodedData);
    }

    public static TimeSpan DecodeTaskData(TaskData taskData)
    {
        var encodedData = taskData.GetData();
        var milliseconds = BitConverter.ToInt32(encodedData);
        return TimeSpan.FromMilliseconds(milliseconds);
    }
}