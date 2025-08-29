using System.Text;
using Base;
using RabbitMQ.Client.Events;

namespace HelloWorld.Receive;

public class MessageReceiver(
    ReceiverOptions options,
    Encoding encoding) 
    : ReceiverBase<string>(options)
{
    protected override async Task OnReceived(object sender, BasicDeliverEventArgs eventArguments)
    {
        var body = eventArguments.Body.ToArray();
        var message = encoding.GetString(body);
        await RegisterAttemptedData(message, true, eventArguments);
    }
}