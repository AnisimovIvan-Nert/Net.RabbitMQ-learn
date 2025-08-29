using Base.Service.Services;

namespace Tests.Fakes;

public class DelaySourceFake : IDelaySource
{
    public TimeSpan Delay { get; } = TimeSpan.FromMilliseconds(100);

    public ValueTask<TimeSpan> GetDealy()
    {
        return ValueTask.FromResult(Delay);
    }
}