namespace Zentient.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static ConfigurationBuilder UseFile(this ConfigurationBuilder builder, string filePath)
    {
        var jsonProvider = new JsonConfigurationProvider(GetFullFilePath(builder, filePath));
        jsonProvider.Load(builder);
        return builder;
    }

    private static string GetFullFilePath(ConfigurationBuilder builder, string filePath)
    {
        var basePath = builder.Get<string>("BasePath") ?? "";
        return Path.IsPathRooted(filePath) ? filePath : Path.Combine(basePath, filePath);
    }

    public static ConfigurationBuilder SetBasePath(this ConfigurationBuilder builder, string basePath)
    {
        builder.WithSetting("BasePath", basePath);
        return builder;
    }
}
