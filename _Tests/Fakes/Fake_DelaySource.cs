using Base.Service.DelaySource;

namespace _Tests.Fakes;

public class DelaySourceFake : IDelaySource
{
    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(100);

    public ValueTask<TimeSpan> GetDealy()
    {
        return ValueTask.FromResult(Delay);
    }
}