using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace _Tests.Utilities;

public static class WebApplicationFactoryExtensions
{
    public static ServiceAccess<TEntryPoint> GetServiceAccess<TEntryPoint>(
        this WebApplicationFactory<TEntryPoint> applicationFactory)
        where TEntryPoint : class
    {
        return new ServiceAccess<TEntryPoint>(applicationFactory);
    }
}

public class ServiceAccess<TEntryPoint>(WebApplicationFactory<TEntryPoint> applicationFactory)
    where TEntryPoint : class
{
    public TImplementation GetService<TService, TImplementation>() where TImplementation : TService
    {
        var service = applicationFactory.Services.GetService<TService>();
        if (service is not TImplementation serviceImplementation)
            throw new InvalidOperationException();
        return serviceImplementation;
    }
}