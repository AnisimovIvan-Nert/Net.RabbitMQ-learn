namespace HelloWorld.Receive.Service.MessageStore;

public interface IMessageStore
{
    ValueTask AddAsync(string message);
}