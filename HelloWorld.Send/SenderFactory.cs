using System.Text;

namespace HelloWorld.Send;

public static class SenderFactory
{
    public static async ValueTask<Sender> CreateAsync(string connectionString, string queue, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        var sender = new Sender(connectionString, queue, encoding);
        await sender.InitializeAsync();
        return sender;
    }
}