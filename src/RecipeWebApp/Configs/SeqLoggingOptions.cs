using Serilog.Events;

namespace RecipeWebApp.Configs
{
    public class SeqLoggingOptions
    {
        public bool Enable { get; set; }
        public string ServerUrl { get; set; }
        public string ApiKey { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
    }
}
