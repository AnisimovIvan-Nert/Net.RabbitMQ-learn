namespace Base.Service.Services;

public interface IDelaySource
{
    ValueTask<TimeSpan> GetDealy();
}