using Serilog;

namespace RecipeWebApp.Configs
{
    public class FileLoggingOptions
    {
        public bool Enable { get; set; }
        public string Path { get; set; }
        public RollingInterval RollingInterval { get; set; }
        public string OutputTemplate { get; set; }
        public long FileSizeLimitBytes { get; set; }
        public int RetainedFileCountLimit { get; set; }
    }
}
