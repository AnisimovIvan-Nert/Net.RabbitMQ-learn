using HelloWorld.Send.Tests.Fakes;
using HelloWorld.Send.Worker;
using HelloWorld.Send.Worker.MessageSource;
using HelloWorld.Worker;
using HelloWorld.Worker.DelaySource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Send.Tests.WorkersApplicationFactories;

public class SenderWorkerApplicationFactory : WebApplicationFactory<Program>
{
    private RabbitMqConnection? _rabbitMqConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (_rabbitMqConnection == null)
            throw new InvalidOperationException();

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(_rabbitMqConnection.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IMessageSource, MessageSourceFake>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }

    public void AddRabbitMqConnection(RabbitMqConnection connection)
    {
        _rabbitMqConnection = connection;
    }
}