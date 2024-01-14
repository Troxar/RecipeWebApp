using Microsoft.AspNetCore.Mvc;
using RecipeWebApp.Filters;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Controllers
{
    [ApiController]
    [Route("api/recipe")]
    [RecipeApiEnabled]
    [RecipeApiHandleException]
    public class RecipeApiController : ControllerBase
    {
        private readonly IRecipeService _service;

        public RecipeApiController(IRecipeService service)
        {
            _service = service;
        }

        [HttpGet("{id:required}")]
        [AddLastModifiedHeader]
        public async Task<IActionResult> Get(int id)
        {
            var recipe = await _service.GetRecipe(id);

            if (recipe is null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(recipe);
        }

        [HttpPost("{id:required}")]
        public async Task<IActionResult> Update(int id, EditRecipeBase editBase)
        {
            var cmd = new UpdateRecipeCommand(id, editBase);
            await _service.UpdateRecipe(cmd);

            return Ok();
        }
    }
}
