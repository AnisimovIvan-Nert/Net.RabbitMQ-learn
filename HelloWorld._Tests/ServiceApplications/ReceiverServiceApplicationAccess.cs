using Base.Service.Configurations;
using Base.Service.Services;
using HelloWorld.Receive.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.Fakes;
using Tests.Utilities;

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
    private RabbitMqOptions? _rabbitMqOptions;

    private RabbitMqOptions RabbitMqOptions => _rabbitMqOptions 
                                                  ?? throw new InvalidOperationException();
    
    public void Initialize(RabbitMqOptions options)
    {
        if (_rabbitMqOptions != null)
            throw new InvalidOperationException();
        
        _rabbitMqOptions = options;
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
            configurationBuilder.AddInMemoryCollection(RabbitMqOptions.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IDataStore<string>, DataStoreFake<string>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}