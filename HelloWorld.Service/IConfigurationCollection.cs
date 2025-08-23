namespace HelloWorld.Service;

public interface IConfigurationCollection
{
    IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection();
}