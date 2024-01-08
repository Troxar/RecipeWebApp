namespace RecipeWebApp.Services.Exceptions
{
    public class RecipeIsDeletedException : RecipeException
    {
        public RecipeIsDeletedException() { }

        public RecipeIsDeletedException(string? message) : base(message) { }

        public RecipeIsDeletedException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
