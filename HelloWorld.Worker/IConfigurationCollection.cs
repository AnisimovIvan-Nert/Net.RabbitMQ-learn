namespace HelloWorld.Worker;

public interface IConfigurationCollection
{
    IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection();
}