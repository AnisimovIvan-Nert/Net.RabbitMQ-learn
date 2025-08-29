using System.Text;
using Base;
using Base.Sender;

namespace HelloWorld.Send;

public class MessageSender(
    SenderOptions options,
    Encoding encoding) 
    : SenderBase<string>(options)
{
    protected override byte[] EncodeData(string data)
    {
        return encoding.GetBytes(data);
    }
}