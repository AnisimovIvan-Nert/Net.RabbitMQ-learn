using HelloWorld.Send.Service.MessageSource;

namespace HelloWorld.Send.Tests.Fakes;

public class MessageSourceFake : IMessageSource
{
    private readonly List<string> _messages = [];

    public void Push(params string[] messages)
    {
        _messages.AddRange(messages);
    }

    public ValueTask<IEnumerable<string>> Pull()
    {
        var result = new List<string>(_messages);
        _messages.Clear();
        return ValueTask.FromResult(result.AsEnumerable());
    }
}