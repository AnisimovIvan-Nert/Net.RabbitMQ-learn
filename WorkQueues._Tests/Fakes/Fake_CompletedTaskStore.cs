using WorkQueues.Worker.Service.CompletedTaskCountStore;

namespace WorkQueues.Tests.Fakes;

public class CompletedTaskStoreFake : ICompletedTaskStore
{
    private readonly List<TaskData> _store = [];

    public IReadOnlyCollection<TaskData> Store => _store;

    public ValueTask AddAsync(TaskData message)
    {
        _store.Add(message);
        return ValueTask.CompletedTask;
    }
}