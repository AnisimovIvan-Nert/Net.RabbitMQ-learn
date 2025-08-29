using Testcontainers.RabbitMq;

namespace Tests.DockerContainers.RabbitMq;

public class RabbitMqFixture : IDisposable
{
    private readonly RabbitMqContainer _rabbitMqContainer = RabbitMqContainerUtilities.RunContainer();

    public void Dispose()
    {
        _rabbitMqContainer.DisposeAsync();
    }

    public string GetConnectionString()
    {
        return _rabbitMqContainer.GetConnectionString();
    }
}