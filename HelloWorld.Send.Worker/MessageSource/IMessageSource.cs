namespace HelloWorld.Send.Worker.MessageSource;

public interface IMessageSource
{
    ValueTask<IEnumerable<string>> Pull();
}