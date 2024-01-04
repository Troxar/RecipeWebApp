using Moq;
using RecipeWebApp.Entities;
using RecipeWebApp.Infrastructure;
using RecipeWebApp.Services;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Tests
{
    public class RecipeServiceTests
    {
        [Fact]
        public async Task CreateRecipe_ShouldAddRecipeToDatabase()
        {
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes.Add(It.IsAny<Recipe>()));
            mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var service = new RecipeService(mockDbContext.Object);
            var command = new CreateRecipeCommand();
            var result = await service.CreateRecipe(command);

            mockDbContext.Verify(m => m.Recipes.Add(It.IsAny<Recipe>()), Times.Once);
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRecipe_ShouldReturnRecipeId()
        {
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var lastId = 123;
            mockDbContext.Setup(c => c.Recipes.Add(It.IsAny<Recipe>()))
                .Callback<Recipe>(r => r.RecipeId = ++lastId);

            var service = new RecipeService(mockDbContext.Object);
            var command = new CreateRecipeCommand();
            var result1 = await service.CreateRecipe(command);
            var result2 = await service.CreateRecipe(command);

            Assert.Equal(124, result1);
            Assert.Equal(125, result2);
        }
    }
}
