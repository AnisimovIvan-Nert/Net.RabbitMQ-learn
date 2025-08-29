using Base.Service.Configurations;
using Base.Service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.Fakes;
using Tests.Utilities;
using WorkQueues.Tests.Fakes;
using WorkQueues.Worker.Service;

namespace WorkQueues.Tests.ServicesApplicationFactories;

public class WorkerServiceApplicationAccess(
    DataStoreFake<TaskData> taskStore, 
    DelaySourceFake delaySource)
{
    public DataStoreFake<TaskData> TaskStore => taskStore;
    public DelaySourceFake DelaySource => delaySource;
}

public class WorkerServiceApplicationFactory : WebApplicationFactory<Program>
{
    private RabbitMqOptions? _rabbitMqOptions;

    private RabbitMqOptions RabbitMqOptions => _rabbitMqOptions 
                                                  ?? throw new InvalidOperationException();
    
    public void Initialize(RabbitMqOptions rabbitMqOptions)
    {
        if (_rabbitMqOptions != null)
            throw new InvalidOperationException();
        
        _rabbitMqOptions = rabbitMqOptions;
    }
    
    public WorkerServiceApplicationAccess GetApplicationAccess()
    {
        var serviceAccess = this.GetServiceAccess();
        var messageStore = serviceAccess.GetService<IDataStore<TaskData>, DataStoreFake<TaskData>>();
        var delaySource = serviceAccess.GetService<IDelaySource, DelaySourceFake>();
        return new WorkerServiceApplicationAccess(messageStore, delaySource);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(RabbitMqOptions.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<ITaskFactory, DelayedTaskFakeFactory>();
            collection.AddSingleton<IDataStore<TaskData>, DataStoreFake<TaskData>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}