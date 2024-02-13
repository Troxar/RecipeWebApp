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

            var loggingOptions = context.Configuration
                .GetSection("FileLogging").Get<FileLoggingOptions>();

            if (loggingOptions?.Enable == true)
            {
                configuration.WriteTo.File(
                    path: loggingOptions.Path,
                    rollingInterval: loggingOptions.RollingInterval,
                    outputTemplate: loggingOptions.OutputTemplate,
                    fileSizeLimitBytes: loggingOptions.FileSizeLimitBytes,
                    retainedFileCountLimit: loggingOptions.RetainedFileCountLimit
                );
            }
        }
    }
}
