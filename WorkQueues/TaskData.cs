namespace WorkQueues;

public readonly struct TaskData
{
    private readonly byte[] _data;
    
    public TaskData(byte[] data)
    {
        _data = data;
    }

    public byte[] GetData() => _data;
}