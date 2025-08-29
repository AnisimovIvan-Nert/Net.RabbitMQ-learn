using Base;
using Base.Sender;

namespace WorkQueues.Sender;

public class TaskSender(SenderOptions options) : SenderBase<TaskData>(options)
{
    protected override byte[] EncodeData(TaskData data)
    {
        return data.GetData();
    }
}