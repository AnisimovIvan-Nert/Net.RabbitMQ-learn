using System.Text;
using Base;

namespace HelloWorld.Send;

public class MessageSender(
    string connectionString, 
    string queue, 
    Encoding encoding) 
    : SenderBase<string>(connectionString, queue)
{
    protected override byte[] EncodeData(string data)
    {
        return encoding.GetBytes(data);
    }
}