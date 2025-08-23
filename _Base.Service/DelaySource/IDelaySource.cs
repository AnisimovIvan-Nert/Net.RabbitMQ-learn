namespace Base.Service.DelaySource;

public interface IDelaySource
{
    ValueTask<TimeSpan> GetDealy();
}