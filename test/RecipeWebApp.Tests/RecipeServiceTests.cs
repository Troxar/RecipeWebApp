using Moq;
using Moq.EntityFrameworkCore;
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

        [Fact]
        public async Task GetRecipes_ShouldNotReturnRecipesIfTheyDoNotExist()
        {
            var recipes = new List<Recipe>();
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipes();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecipes_ShouldReturnExpectedRecipes()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 2, Name = "Recipe 2" },
                new Recipe { RecipeId = 3, Name = "Recipe 3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipes();

            Assert.NotNull(result);
            Assert.Equal(recipes.Count, result.Count());

            foreach (var recipe in recipes)
            {
                Assert.Contains(result, r => r.Id == recipe.RecipeId && r.Name == recipe.Name);
            }
        }

        [Fact]
        public async Task GetRecipes_ShouldOnlyReturnNonDeletedRecipes()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1", IsDeleted = false },
                new Recipe { RecipeId = 2, Name = "Recipe 2", IsDeleted = true },
                new Recipe { RecipeId = 3, Name = "Recipe 3", IsDeleted = false }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);
            var expected = recipes.Where(r => !r.IsDeleted).ToArray();

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipes();

            Assert.NotNull(result);
            Assert.Equal(expected.Length, result.Count());

            foreach (var recipe in expected)
            {
                Assert.Contains(result, r => r.Id == recipe.RecipeId && r.Name == recipe.Name);
            }
        }

        [Fact]
        public async Task GetRecipes_ShouldNotReturnRecipesIfTheyAreDeleted()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1", IsDeleted = true },
                new Recipe { RecipeId = 2, Name = "Recipe 2", IsDeleted = true },
                new Recipe { RecipeId = 3, Name = "Recipe 3", IsDeleted = true }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipes();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecipe_ShouldReturnNullIfRecipeNotFound()
        {
            var recipes = new List<Recipe>()
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 2;
            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipe(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRecipe_ShouldThrowExceptionIfThereAreSeveralRecipesWithSameId()
        {
            var recipes = new List<Recipe>()
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1", Ingredients = Array.Empty<Ingredient>() },
                new Recipe { RecipeId = 1, Name = "Recipe 2", Ingredients = Array.Empty<Ingredient>() }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 1;
            var service = new RecipeService(mockDbContext.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetRecipe(id));
        }

        [Fact]
        public async Task GetRecipe_ShouldReturnRecipeWithIngredients()
        {
            var recipes = new List<Recipe>()
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1", Ingredients = new []
                    {
                        new Ingredient { Name = "11" },
                        new Ingredient { Name = "12" },
                    }
                },
                new Recipe { RecipeId = 2, Name = "Recipe 2", Ingredients = new []
                    {
                        new Ingredient { Name = "21" },
                        new Ingredient { Name = "22" },
                    }
                },
                new Recipe { RecipeId = 3, Name = "Recipe 3", Ingredients = new []
                    {
                        new Ingredient { Name = "31" },
                        new Ingredient { Name = "32" },
                    }
                },
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 2;
            var expected = recipes.Where(r => r.RecipeId == id).First();

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipe(id);

            Assert.NotNull(result);
            Assert.Equal(expected.RecipeId, result.Id);
            Assert.Equal(expected.Name, result.Name);

            Assert.NotNull(result.Ingredients);
            Assert.Equal(expected.Ingredients.Count, result.Ingredients.Count());

            foreach (var ingredient in expected.Ingredients)
            {
                Assert.Contains(result.Ingredients, i => i.Name == ingredient.Name);
            }
        }
    }
}
