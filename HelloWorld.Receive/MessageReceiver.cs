using System.Text;
using _Base;
using RabbitMQ.Client.Events;

namespace HelloWorld.Receive;

public class MessageReceiver(
    string connectionString, 
    string queue, 
    Encoding encoding) 
    : ReceiverBase<string>(connectionString, queue)
{
    protected override Task OnReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = encoding.GetString(body);
        RegisterHandledData(message);
        return Task.CompletedTask;
    }
}