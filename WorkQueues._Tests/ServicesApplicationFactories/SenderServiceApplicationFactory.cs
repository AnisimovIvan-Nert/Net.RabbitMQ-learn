using Base.Service.Configurations;
using Base.Service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.Fakes;
using Tests.Utilities;
using WorkQueues.Sender.Service;

namespace WorkQueues.Tests.ServicesApplicationFactories;

public class SenderServiceApplicationAccess(
    DataSourceFake<TaskData> taskSource,
    DelaySourceFake delaySource)
{
    public DataSourceFake<TaskData> TaskSource => taskSource;
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
        var messageSource = serviceAccess.GetService<IDataSource<TaskData>, DataSourceFake<TaskData>>();
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
            collection.AddSingleton<IDataSource<TaskData>, DataSourceFake<TaskData>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}