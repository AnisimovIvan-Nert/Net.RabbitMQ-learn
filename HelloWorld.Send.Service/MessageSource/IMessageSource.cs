namespace HelloWorld.Send.Service.MessageSource;

public interface IMessageSource
{
    ValueTask<IEnumerable<string>> Pull();
}