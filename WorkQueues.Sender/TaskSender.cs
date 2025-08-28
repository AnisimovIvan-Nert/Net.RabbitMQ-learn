using _Base;

namespace WorkQueues.Sender;

public class TaskSender(
    string connectionString, 
    string queue) 
    : SenderBase<TaskData>(connectionString, queue)
{
    protected override byte[] EncodeData(TaskData data)
    {
        return data.GetData();
    }
}