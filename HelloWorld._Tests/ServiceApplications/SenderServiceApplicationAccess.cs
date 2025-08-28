using Base.Service;
using Base.Service.Services;
using HelloWorld.Send.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.Fakes;
using Tests.Utilities;

namespace HelloWorld.Send.Tests.ServiceApplications;

public class SenderServiceApplicationAccess(
    DataSourceFake<string> messageSource, 
    DelaySourceFake delaySource)
{
    public DataSourceFake<string> MessageSource => messageSource;
    public DelaySourceFake DelaySource => delaySource;
}


public class SenderServiceApplicationFactory : WebApplicationFactory<Program>
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
    
    public SenderServiceApplicationAccess GetApplicationAccess()
    {
        var serviceAccess = this.GetServiceAccess();
        var messageSource = serviceAccess.GetService<IDataSource<string>, DataSourceFake<string>>();
        var delaySource = serviceAccess.GetService<IDelaySource, DelaySourceFake>();
        return new SenderServiceApplicationAccess(messageSource, delaySource);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(RabbitMqConnection.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IDataSource<string>, DataSourceFake<string>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}