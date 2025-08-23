using HelloWorld.Receive.Service.MessageStore;

namespace HelloWorld.Send.Tests.Fakes;

public class MessageStoreFake : IMessageStore
{
    private readonly List<string> _store = [];

    public IReadOnlyCollection<string> Store => _store;

    public ValueTask AddAsync(string message)
    {
        _store.Add(message);
        return ValueTask.CompletedTask;
    }
}