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
    public class RecipeApiController : ControllerBase
    {
        private readonly IRecipeService _service;
        private readonly ILogger<RecipeApiController> _logger;

        public RecipeApiController(IRecipeService service, ILogger<RecipeApiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{id:required}")]
        public async Task<IActionResult> Get(int id)
        {
            RecipeDetailViewModel? recipe;

            try
            {
                recipe = await _service.GetRecipe(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe: {id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to get recipe: {id}");
            }

            if (recipe is null)
            {
                return BadRequest($"Recipe not found: {id}");
            }

            return new JsonResult(recipe);
        }

        [HttpPost("{id:required}")]
        public async Task<IActionResult> Update(int id, EditRecipeBase editBase)
        {
            var cmd = new UpdateRecipeCommand(id, editBase);

            try
            {
                await _service.UpdateRecipe(cmd);
            }
            catch (RecipeException ex)
            {
                _logger.LogWarning(ex, "Failed to get recipe for updating: {id}", id);
                return ex is RecipeNotFoundException
                        ? NotFound(ex.Message)
                        : BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update recipe: {id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to update recipe: {id}");
            }

            return Ok();
        }
    }
}
