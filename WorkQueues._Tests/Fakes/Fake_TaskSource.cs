using WorkQueues.Sender.Service.TaskSource;

namespace WorkQueues.Tests.Fakes;

public class TaskSourceFake : ITaskSource
{
    private readonly List<TaskData> _tasks = [];

    public void Push(params TaskData[] messages)
    {
        _tasks.AddRange(messages);
    }

    public ValueTask<IEnumerable<TaskData>> Pull()
    {
        var result = new List<TaskData>(_tasks);
        _tasks.Clear();
        return ValueTask.FromResult(result.AsEnumerable());
    }
}