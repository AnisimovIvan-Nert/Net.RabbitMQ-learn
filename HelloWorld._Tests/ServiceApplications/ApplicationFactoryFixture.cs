namespace HelloWorld.Send.Tests.ServiceApplications;

public class ApplicationFactoryFixture<TApplicationFactory>
    where TApplicationFactory : new()
{
    public TApplicationFactory CreateFactory()
    {
        return new TApplicationFactory();
    }
}