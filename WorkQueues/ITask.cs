namespace WorkQueues;

public interface ITask
{
    public ValueTask<bool> Execute();
}