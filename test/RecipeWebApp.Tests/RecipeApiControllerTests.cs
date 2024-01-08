using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeWebApp.Controllers;
using RecipeWebApp.Services;
using RecipeWebApp.Services.Exceptions;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Tests
{
    public class RecipeApiControllerServiceTests
    {
        #region Get

        [Fact]
        public async Task Get_ShouldReturnJsonResultIfRecipeExists()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.GetRecipe(It.IsAny<int>())).ReturnsAsync(new RecipeDetailViewModel());
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Get(1);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequestIfRecipeNotFound()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.GetRecipe(It.IsAny<int>())).ReturnsAsync((RecipeDetailViewModel?)null);
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Get(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Get_ShouldReturnObjectResult500IfExceptionOccurs()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.GetRecipe(It.IsAny<int>())).Throws<Exception>();
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Get(1);

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldReturnOkIfRecipeExists()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.UpdateRecipe(It.IsAny<UpdateRecipeCommand>()));
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Update(1, new EditRecipeBase());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFoundIfRecipeDoesNotExist()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.UpdateRecipe(It.IsAny<UpdateRecipeCommand>())).Throws<RecipeNotFoundException>();
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Update(1, new EditRecipeBase());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequestIfRecipeIsDeleted()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.UpdateRecipe(It.IsAny<UpdateRecipeCommand>())).Throws<RecipeIsDeletedException>();
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Update(1, new EditRecipeBase());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnObjectResult500IfExceptionOccurs()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.UpdateRecipe(It.IsAny<UpdateRecipeCommand>())).Throws<Exception>();
            var mockLogger = new Mock<ILogger<RecipeApiController>>();
            var controller = new RecipeApiController(mockService.Object, mockLogger.Object);

            var result = await controller.Update(1, new EditRecipeBase());

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }

        #endregion
    }
}
