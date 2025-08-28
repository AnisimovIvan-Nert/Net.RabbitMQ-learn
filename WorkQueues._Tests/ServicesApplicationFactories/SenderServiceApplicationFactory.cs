using Base.Service;
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
        var messageSource = serviceAccess.GetService<IDataSource<TaskData>, DataSourceFake<TaskData>>();
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
            collection.AddSingleton<IDataSource<TaskData>, DataSourceFake<TaskData>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}