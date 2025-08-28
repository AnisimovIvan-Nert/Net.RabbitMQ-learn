namespace Tests.Fixtures;

public class ApplicationFactoryFixture<TApplicationFactory>
    where TApplicationFactory : new()
{
    public TApplicationFactory CreateFactory()
    {
        return new TApplicationFactory();
    }
}