using System.Text;

namespace HelloWorld.Receive;

public static class ReceiverFactory
{
    public static async ValueTask<Receiver> CreateAsync(string host, string queue, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var receiver = new Receiver(host, queue, encoding);
        await receiver.InitializeAsync();
        return receiver;
    }
}