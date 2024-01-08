namespace RecipeWebApp.Services.Exceptions
{
    public abstract class RecipeException : ApplicationException
    {
        protected RecipeException() { }

        protected RecipeException(string? message) : base(message) { }

        protected RecipeException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
