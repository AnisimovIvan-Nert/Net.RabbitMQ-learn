using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Base.Service.Configurations;

public static class ConfigurationHelper
{
    public static IEnumerable<KeyValuePair<string, string?>> GetConfigurationPairs(
        IConfigurationCollection configurationCollection,
        string section,
        params IConfigurationCollection[] nestedConfigurations)
    {
        foreach (var configurationPair in GetPrimitiveConfigurationPairs(configurationCollection, section))
            yield return configurationPair;

        foreach (var configurationPair in GetNestedConfigurationPairs(section, nestedConfigurations))
            yield return configurationPair;
    }
    
    public static IEnumerable<KeyValuePair<string, string?>> GetPrimitiveConfigurationPairs(
        IConfigurationCollection configuration, 
        string section)
    {
        var optionProperties = configuration.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsPrimitive || property.PropertyType == typeof(string));
        var optionNameValuePairs = optionProperties
            .Select(property => new KeyValuePair<string, object?>(property.Name, property.GetValue(configuration)));
        
        foreach (var pair in optionNameValuePairs)
            yield return new KeyValuePair<string, string?>(section + ':' + pair.Key, pair.Value?.ToString());
    }
    
    public static IEnumerable<KeyValuePair<string, string?>> GetNestedConfigurationPairs(
        string section,
        params IConfigurationCollection[] nestedConfigurations)
    {
        foreach (var nestedConfiguration in nestedConfigurations)
        {
            foreach (var configuration in nestedConfiguration.GetConfigurationCollection())
            {
                yield return new KeyValuePair<string, string?>(section + ':' + configuration.Key, configuration.Value);

            }
        }
    }
    
    public static void Configure<TOption>(
        HostApplicationBuilder builder,
        string section,
        string parentSection = "", 
        params Action<HostApplicationBuilder, string>[] nestedConfigurations)
        where TOption : class
    {
        if (parentSection != "")
            section = parentSection + ':' + section;
        
        builder.Services.Configure<TOption>(builder.Configuration.GetSection(section));
        
        foreach (var nestedConfigurationAction in nestedConfigurations)
            nestedConfigurationAction(builder, section);
    }
}