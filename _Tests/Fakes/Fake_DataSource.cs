using Base.Service.Services;

namespace Tests.Fakes;

public class DataSourceFake<T> : IDataSource<T>
{
    private readonly List<T> _messages = [];

    public void Push(params T[] messages)
    {
        _messages.AddRange(messages);
    }

    public ValueTask<IEnumerable<T>> Pull()
    {
        var result = new List<T>(_messages);
        _messages.Clear();
        return ValueTask.FromResult(result.AsEnumerable());
    }
}