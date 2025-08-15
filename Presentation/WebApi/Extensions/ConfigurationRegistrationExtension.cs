using Serilog;

namespace WebApi.Extensions;

public static class ConfigurationRegistrationExtension
{
    public static WebApplicationBuilder AddCustomJsonConfigurations(this WebApplicationBuilder builder)
    {
        var configFolder = builder.Configuration.GetValue<string>("Config");
        configFolder = string.IsNullOrWhiteSpace(configFolder) || configFolder == "__CONFIG_PATH__"
            ? "Configuration"
            : configFolder;

        if (!Directory.Exists(configFolder))
        {
            Log.Logger.Error("ICCParis startup failed: Configuration directory '{ConfigFolder}' does not exist (check 'Config' in appsettings.json).", configFolder);
            Log.CloseAndFlush();
            throw new InvalidOperationException(
                $"ICCParis startup failed: Configuration directory '{configFolder}' does not exist (check 'Config' in appsettings.json).");
        }

        string[] configFiles =
        [
            "cors.json",
            "database.json",
            "logging.json",
            "serilog.json",
        ];

        foreach (string file in configFiles)
        {
            builder.Configuration.AddJsonFile(Path.Combine(configFolder, file), optional: false, reloadOnChange: true);
        }

        return builder;
    }
}
