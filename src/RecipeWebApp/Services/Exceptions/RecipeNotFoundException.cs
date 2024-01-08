namespace RecipeWebApp.Services.Exceptions
{
    public class RecipeNotFoundException : RecipeException
    {
        public RecipeNotFoundException() { }

        public RecipeNotFoundException(string? message) : base(message) { }

        public RecipeNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
