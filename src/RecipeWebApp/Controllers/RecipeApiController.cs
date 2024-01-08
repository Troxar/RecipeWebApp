using Microsoft.AspNetCore.Mvc;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Controllers
{
    [Route("api/recipe")]
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
                return NotFound($"Failed to get recipe: {id}");
            }

            if (recipe is null)
            {
                return BadRequest($"Recipe not found: {id}");
            }

            return Ok(recipe);
        }
    }
}
