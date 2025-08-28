using _Tests.Fakes;
using _Tests.Utilities;
using Base.Service;
using Base.Service.Services;
using HelloWorld.Receive.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Send.Tests.ServiceApplications;

public class ReceiverServiceApplicationAccess(
    DataStoreFake<string> messageStore, 
    DelaySourceFake delaySource)
{
    public DataStoreFake<string> MessageStore => messageStore;
    public DelaySourceFake DelaySource => delaySource;
}

public class ReceiverServiceApplicationFactory : WebApplicationFactory<Program>
{
    private RabbitMqConnection? _rabbitMqConnection;

    private RabbitMqConnection RabbitMqConnection => _rabbitMqConnection 
                                                     ?? throw new InvalidOperationException();
    
    public void Initialize(RabbitMqConnection connection)
    {
        if (_rabbitMqConnection != null)
            throw new InvalidOperationException();
        
        _rabbitMqConnection = connection;
    }

    public ReceiverServiceApplicationAccess GetApplicationAccess()
    {
        var serviceAccess = this.GetServiceAccess();
        var messageStore = serviceAccess.GetService<IDataStore<string>, DataStoreFake<string>>();
        var delaySource = serviceAccess.GetService<IDelaySource, DelaySourceFake>();
        return new ReceiverServiceApplicationAccess(messageStore, delaySource);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(RabbitMqConnection.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IDataStore<string>, DataStoreFake<string>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}