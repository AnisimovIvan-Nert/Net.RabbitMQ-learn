using Microsoft.Extensions.Hosting;

namespace Base.Service.Configurations;

public interface IConfigurationCollection
{
    IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection();
}