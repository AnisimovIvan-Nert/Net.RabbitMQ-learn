namespace Base.Service;

public interface IConfigurationCollection
{
    IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection();
}