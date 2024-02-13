using Microsoft.AspNetCore.Mvc;
using RecipeWebApp.Filters;
using RecipeWebApp.Services;
using RecipeWebApp.Services.Exceptions;
using RecipeWebApp.ViewModels;
using System.Net;

namespace RecipeWebApp.Controllers
{
    [ApiController]
    [Route("api/recipe")]
    [RecipeApiEnabled]
    [RecipeApiHandleException]
    public class RecipeApiController : ControllerBase
    {
        private readonly IRecipeService _service;
        private readonly ILogger<RecipeApiController> _logger;

        public RecipeApiController(IRecipeService service,
            ILogger<RecipeApiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{id:required}")]
        [AddLastModifiedHeader]
        public async Task<IActionResult> Get(int id)
        {
            var recipe = await _service.GetRecipe(id);
            if (recipe is null)
            {
                _logger.LogWarning("Recipe not found: {RecipeId}", id);
                return NotFound();
            }

            _logger.LogInformation("Recipe loaded: {RecipeId}", id);
            return new JsonResult(recipe);
        }

        [HttpPost("{id:required}")]
        public async Task<IActionResult> Update(int id, EditRecipeBase editBase)
        {
            var cmd = new UpdateRecipeCommand(id, editBase);

            try
            {
                await _service.UpdateRecipe(cmd);
                _logger.LogInformation("Recipe updated: {RecipeId}", id);
                return Ok();
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to update recipe: {RecipeId}", id);
                return NotFound();
            }
            catch (RecipeIsDeletedException ex)
            {
                _logger.LogWarning(ex, "Failed to update recipe: {RecipeId}", id);
                return StatusCode((int)HttpStatusCode.Gone, "Recipe has been deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update recipe: {RecipeId}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
