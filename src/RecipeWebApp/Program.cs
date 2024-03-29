using RecipeWebApp.Configs;
using Serilog;

namespace RecipeWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(ConfigureSerilog)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseIIS();
                    webBuilder.UseIISIntegration();
                });

        private static void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration configuration)
        {
            configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            ConfigureFileLogging(context.Configuration, configuration);
            ConfigureSeqLogging(context.Configuration, configuration);
        }

        private static void ConfigureFileLogging(IConfiguration config, LoggerConfiguration loggerConfig)
        {
            var fileLoggingOptions = config.GetSection("FileLogging").Get<FileLoggingOptions>();

            if (fileLoggingOptions?.Enable == true)
            {
                loggerConfig.WriteTo.File(
                    path: fileLoggingOptions.Path,
                    rollingInterval: fileLoggingOptions.RollingInterval,
                    outputTemplate: fileLoggingOptions.OutputTemplate,
                    fileSizeLimitBytes: fileLoggingOptions.FileSizeLimitBytes,
                    retainedFileCountLimit: fileLoggingOptions.RetainedFileCountLimit
                );
            }
        }

        private static void ConfigureSeqLogging(IConfiguration config, LoggerConfiguration loggerConfig)
        {
            var seqLoggingOptions = config.GetSection("SeqLogging").Get<SeqLoggingOptions>();

            if (seqLoggingOptions?.Enable == true)
            {
                loggerConfig.WriteTo.Seq(
                    serverUrl: seqLoggingOptions.ServerUrl,
                    apiKey: seqLoggingOptions.ApiKey,
                    restrictedToMinimumLevel: seqLoggingOptions.MinimumLevel
                );
            }
        }
    }
}
