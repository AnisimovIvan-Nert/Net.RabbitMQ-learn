using Testcontainers.RabbitMq;

namespace Tests.DockerContainers.RabbitMq;

internal static class RabbitMqContainerUtilities
{
    private const string ImageName = "rabbitmq";

    public static async ValueTask<RabbitMqContainer> RunContainer()
    {
        var rabbitMqContainer = new RabbitMqBuilder()
            .WithImage(ImageName)
            .WithAutoRemove(true)
            .Build();

        await rabbitMqContainer.StartAsync();
        return rabbitMqContainer;
    }
}