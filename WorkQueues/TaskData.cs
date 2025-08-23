namespace WorkQueues;

public readonly struct TaskData
{
    private readonly byte[] _data;

    public TaskData(TimeSpan executionTime)
    {
        _data = BitConverter.GetBytes(executionTime.Milliseconds);
    }

    public TaskData(byte[] data)
    {
        _data = data;
    }

    public TimeSpan GetExecutionTime()
    {
        var milliseconds = BitConverter.ToInt32(_data);
        return TimeSpan.FromMilliseconds(milliseconds);
    }

    public byte[] GetData() => _data;
}