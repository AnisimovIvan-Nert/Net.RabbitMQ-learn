using Base.Service.Configurations;
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
    private RabbitMqOptions? _rabbitMqOptions;

    private RabbitMqOptions RabbitMqOptions => _rabbitMqOptions 
                                                  ?? throw new InvalidOperationException();
    
    public void Initialize(RabbitMqOptions options)
    {
        if (_rabbitMqOptions != null)
            throw new InvalidOperationException();
        
        _rabbitMqOptions = options;
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
            configurationBuilder.AddInMemoryCollection(RabbitMqOptions.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IDataSource<string>, DataSourceFake<string>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}