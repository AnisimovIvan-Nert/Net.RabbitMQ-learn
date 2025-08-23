namespace HelloWorld.Sen.Service.MessageSource;

public interface IMessageSource
{
    ValueTask<IEnumerable<string>> Pull();
}