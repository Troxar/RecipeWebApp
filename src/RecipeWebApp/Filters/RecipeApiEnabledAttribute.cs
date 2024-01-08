using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RecipeWebApp.Configs;

namespace RecipeWebApp.Filters
{
    public class RecipeApiEnabledAttribute : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<RecipeApiConfig>>();
            if (!options.Value.IsEnabled)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
