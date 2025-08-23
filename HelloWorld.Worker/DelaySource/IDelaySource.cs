namespace HelloWorld.Worker.DelaySource;

public interface IDelaySource
{
    ValueTask<TimeSpan> GetDealy();
}