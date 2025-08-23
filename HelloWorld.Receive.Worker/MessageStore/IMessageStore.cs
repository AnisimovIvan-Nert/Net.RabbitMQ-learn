namespace HelloWorld.Receive.Worker.MessageStore;

public interface IMessageStore
{
    ValueTask AddAsync(string message);
}