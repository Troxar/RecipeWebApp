using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RecipeWebApp.Controllers;
using RecipeWebApp.Services;
using RecipeWebApp.Services.Exceptions;
using RecipeWebApp.ViewModels;
using System.Globalization;
using System.Net;

namespace RecipeWebApp.Tests
{
    public class RecipeApiControllerServiceTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly Mock<IRecipeService> _mockService;
        private readonly HttpClient _client;

        public RecipeApiControllerServiceTests(WebApplicationFactory<Startup> fixture)
        {
            _mockService = new Mock<IRecipeService>();
            var factory = fixture.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IRecipeService>();
                    services.AddSingleton(_mockService.Object);
                });
            });
            _client = factory.CreateClient();
        }

        #region Get

        [Fact]
        public async Task Get_ShouldReturnJsonResultIfRecipeExists()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.GetRecipe(It.IsAny<int>())).ReturnsAsync(new RecipeDetailViewModel());
            var controller = new RecipeApiController(mockService.Object);

            var result = await controller.Get(1);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFoundResultIfRecipeDoesNotExist()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.GetRecipe(It.IsAny<int>())).ReturnsAsync((RecipeDetailViewModel?)null);
            var controller = new RecipeApiController(mockService.Object);

            var result = await controller.Get(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_ShouldReturnHttpMessageWithStatusCode500IfExceptionOccurs()
        {
            _mockService.Setup(m => m.GetRecipe(It.IsAny<int>())).Throws<Exception>();

            var response = await _client.GetAsync("/api/recipe/1");

            Assert.IsType<HttpResponseMessage>(response);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Get_ShouldReturnLastModifiedInHeaderIfDateIsSpecified()
        {
            var detail = new RecipeDetailViewModel { LastModified = DateTime.UtcNow };
            _mockService.Setup(m => m.GetRecipe(It.IsAny<int>())).Returns(Task.FromResult<RecipeDetailViewModel?>(detail));

            var response = await _client.GetAsync("/api/recipe/1");

            Assert.IsType<HttpResponseMessage>(response);
            Assert.True(response.Content.Headers.TryGetValues("Last-Modified", out var values));

            var expected = detail.LastModified.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
            var actual = values.First();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_ShouldNotReturnLastModifiedInHeaderIfDateIsNotSpecified()
        {
            var detail = new RecipeDetailViewModel();
            _mockService.Setup(m => m.GetRecipe(It.IsAny<int>())).Returns(Task.FromResult<RecipeDetailViewModel?>(detail));

            var response = await _client.GetAsync("/api/recipe/1");

            Assert.IsType<HttpResponseMessage>(response);
            Assert.False(response.Content.Headers.TryGetValues("Last-Modified", out var values));
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldReturnOkRestulIfThereAreNoExceptions()
        {
            var mockService = new Mock<IRecipeService>();
            mockService.Setup(s => s.UpdateRecipe(It.IsAny<UpdateRecipeCommand>()));
            var controller = new RecipeApiController(mockService.Object);

            var result = await controller.Update(1, new EditRecipeBase());

            Assert.IsType<OkResult>(result);
        }

        [Theory]
        [InlineData(typeof(RecipeNotFoundException), HttpStatusCode.NotFound)]
        [InlineData(typeof(RecipeIsDeletedException), HttpStatusCode.BadRequest)]
        [InlineData(typeof(Exception), HttpStatusCode.InternalServerError)]
        public async Task Update_ShouldReturnHttpMessageWithStatusCode500IfExceptionOccurs(Type exceptionType, HttpStatusCode statusCode)
        {
            var exception = (Exception)Activator.CreateInstance(exceptionType)!;
            _mockService.Setup(m => m.UpdateRecipe(It.IsAny<UpdateRecipeCommand>())).Throws(exception);

            var response = await _client.PostAsJsonAsync("/api/recipe/1", new EditRecipeBase { Name = "new name" });

            Assert.IsType<HttpResponseMessage>(response);
            Assert.Equal(statusCode, response.StatusCode);
        }

        #endregion
    }
}
