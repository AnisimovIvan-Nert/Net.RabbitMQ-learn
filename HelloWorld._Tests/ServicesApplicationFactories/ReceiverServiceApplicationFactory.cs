using Base.Service;
using Base.Service.DelaySource;
using HelloWorld.Receive.Service;
using HelloWorld.Receive.Service.MessageStore;
using HelloWorld.Send.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Send.Tests.ServicesApplicationFactories;

public class ReceiverServiceApplicationFactory : WebApplicationFactory<Program>
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
            collection.AddSingleton<IMessageStore, MessageStoreFake>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }

    public void AddRabbitMqConnection(RabbitMqConnection connection)
    {
        _rabbitMqConnection = connection;
    }
}