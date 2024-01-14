using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecipeWebApp.ViewModels;
using System.Net;

namespace RecipeWebApp.Filters
{
    public class AddLastModifiedHeaderAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is JsonResult result
                && result.Value is RecipeDetailViewModel detail
                && detail.LastModified != DateTime.MinValue)
            {
                var lastModified = context.HttpContext.Request.GetTypedHeaders().IfModifiedSince;
                if (lastModified.HasValue && lastModified >= detail.LastModified)
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.NotModified);
                }
                context.HttpContext.Response.GetTypedHeaders().LastModified = (DateTimeOffset)detail.LastModified;
            }
        }
    }
}
