namespace HelloWorld.Send;

public static class SenderFactory
{
    public static async ValueTask<Sender> CreateAsync(string connectionString, string queue)
    {
        var sender = new Sender(connectionString, queue);
        await sender.InitializeAsync();
        return sender;
    }
}