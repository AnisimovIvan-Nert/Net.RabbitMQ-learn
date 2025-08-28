using Base.Service;
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
    private RabbitMqConnection? _rabbitMqConnection;

    private RabbitMqConnection RabbitMqConnection => _rabbitMqConnection 
                                                     ?? throw new InvalidOperationException();
    
    public void Initialize(RabbitMqConnection connection)
    {
        if (_rabbitMqConnection != null)
            throw new InvalidOperationException();
        
        _rabbitMqConnection = connection;
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
            configurationBuilder.AddInMemoryCollection(RabbitMqConnection.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<ITaskFactory, TaskFactoryFake>();
            collection.AddSingleton<IDataStore<TaskData>, DataStoreFake<TaskData>>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }
}