namespace HelloWorld.Service.DelaySource;

public interface IDelaySource
{
    ValueTask<TimeSpan> GetDealy();
}