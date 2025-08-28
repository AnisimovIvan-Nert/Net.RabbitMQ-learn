using Base.Service.Services;

namespace _Tests.Fakes;

public class DataStoreFake<T> : IDataStore<T>
{
    private readonly List<T> _store = [];

    public IReadOnlyCollection<T> Store => _store;

    public ValueTask AddAsync(T message)
    {
        _store.Add(message);
        return ValueTask.CompletedTask;
    }
}