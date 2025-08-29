namespace WorkQueues.Tests.Fakes;

public class FailableTaskFake(bool fail) : ITask
{
    public ValueTask<bool> Execute()
    {
        return ValueTask.FromResult(fail == false);
    }
}

public class FailableTaskFakeFactory : ITaskFactory
{
    public ITask Create(TaskData taskData)
    {
        var fail = DecodeTaskData(taskData);
        return new FailableTaskFake(fail);
    }

    public static TaskData EncodeTaskData(bool fail)
    {
        var encodedData = BitConverter.GetBytes(fail);
        return new TaskData(encodedData);
    }

    private static bool DecodeTaskData(TaskData taskData)
    {
        var encodedData = taskData.GetData();
        return BitConverter.ToBoolean(encodedData);
    }
}