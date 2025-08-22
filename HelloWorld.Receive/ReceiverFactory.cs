namespace HelloWorld.Receive;

public static class ReceiverFactory
{
    public static async ValueTask<Receiver> CreateAsync(string host, string queue)
    {
        var receiver = new Receiver(host, queue);
        await receiver.InitializeAsync();
        return receiver;
    }
}