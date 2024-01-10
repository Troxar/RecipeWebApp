using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecipeWebApp.Controllers;
using RecipeWebApp.Services.Exceptions;
using System.Net;

namespace RecipeWebApp.Filters
{
    public class RecipeApiHandleExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            WriteToLog(context);
            SetContextResult(context);
            context.ExceptionHandled = true;
        }

        private void SetContextResult(ExceptionContext context)
        {
            var status = (int)GetStatusCode(context.Exception);
            var error = new ProblemDetails
            {
                Title = "An error occurred",
                Detail = context.Exception.Message,
                Status = status,
                Type = "https://httpstatuses.com/" + status.ToString()
            };
            context.Result = new ObjectResult(error) { StatusCode = status };
        }

        private HttpStatusCode GetStatusCode(Exception ex) => ex switch
        {
            RecipeNotFoundException => HttpStatusCode.NotFound,
            RecipeIsDeletedException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        private void WriteToLog(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RecipeApiController>>();
            var logLevel = GetLogLevel(context.Exception);
            logger.Log(logLevel, context.Exception.Message);
        }

        private LogLevel GetLogLevel(Exception ex) => ex switch
        {
            RecipeException => LogLevel.Warning,
            _ => LogLevel.Error
        };
    }
}
