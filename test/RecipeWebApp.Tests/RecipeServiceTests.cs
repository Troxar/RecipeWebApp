using Moq;
using Moq.EntityFrameworkCore;
using RecipeWebApp.Entities;
using RecipeWebApp.Infrastructure;
using RecipeWebApp.Services;
using RecipeWebApp.Services.Exceptions;
using RecipeWebApp.ViewModels;

namespace RecipeWebApp.Tests
{
    public class RecipeServiceTests
    {
        #region CreateRecipe

        [Fact]
        public async Task CreateRecipe_ShouldAddRecipeToDatabase()
        {
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes.Add(It.IsAny<Recipe>()));
            mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var service = new RecipeService(mockDbContext.Object);
            var command = new CreateRecipeCommand();
            var user = new ApplicationUser();
            var result = await service.CreateRecipe(command, user);

            mockDbContext.Verify(m => m.Recipes.Add(It.IsAny<Recipe>()), Times.Once);
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRecipe_ShouldFillCreatedByIdWithUserId()
        {
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes.Add(It.IsAny<Recipe>()));
            mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var service = new RecipeService(mockDbContext.Object);
            var command = new CreateRecipeCommand();
            var id = Guid.NewGuid().ToString();
            var user = new ApplicationUser { Id = id };
            var result = await service.CreateRecipe(command, user);

            Assert.Equal(id, result.CreatedById);
        }

        #endregion

        #region GetRecipes

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

        #endregion

        #region GetRecipe

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
            var expected = recipes.Where(r => r.RecipeId == id).Single();

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

        #endregion

        #region GetRecipeForUpdate

        [Fact]
        public async Task GetRecipeForUpdate_ShouldReturnNullIfRecipeNotFound()
        {
            var recipes = new List<Recipe>()
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 2;
            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipeForUpdate(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRecipeForUpdate_ShouldThrowExceptionIfThereAreSeveralRecipesWithSameId()
        {
            var recipes = new List<Recipe>()
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 1, Name = "Recipe 2" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 1;
            var service = new RecipeService(mockDbContext.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetRecipeForUpdate(id));
        }

        [Fact]
        public async Task GetRecipeForUpdate_ShouldReturnExpectedRecipe()
        {
            var recipes = new List<Recipe>()
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 2, Name = "Recipe 2" },
                new Recipe { RecipeId = 3, Name = "Recipe 3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 2;
            var expected = recipes.Where(r => r.RecipeId == id).Single();

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipeForUpdate(id);

            Assert.NotNull(result);
            Assert.Equal(expected.RecipeId, result.RecipeId);
            Assert.Equal(expected.Name, result.Name);
        }


        [Fact]
        public async Task GetRecipeForUpdate_ShouldReturnNullIfRecipeIsDeleted()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 2, Name = "Recipe 2", IsDeleted = true },
                new Recipe { RecipeId = 3, Name = "Recipe 3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 2;
            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetRecipeForUpdate(id);

            Assert.Null(result);
        }

        #endregion

        #region UpdateRecipe

        [Fact]
        public async Task UpdateRecipe_ShouldThrowExceptionIfRecipeNotFound()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var cmd = new UpdateRecipeCommand { RecipeId = 2 };
            var service = new RecipeService(mockDbContext.Object);

            await Assert.ThrowsAsync<RecipeNotFoundException>(async () => await service.UpdateRecipe(cmd));
        }

        [Fact]
        public async Task UpdateRecipe_ShouldThrowExceptionIfRecipeIsDeleted()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 2, Name = "Recipe 2", IsDeleted = true },
                new Recipe { RecipeId = 3, Name = "Recipe 3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);
            mockDbContext.Setup(c => c.Recipes.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] id) => recipes.Single(r => r.RecipeId == (int)id[0]));

            var cmd = new UpdateRecipeCommand { RecipeId = 2 };
            var service = new RecipeService(mockDbContext.Object);

            await Assert.ThrowsAsync<RecipeIsDeletedException>(async () => await service.UpdateRecipe(cmd));
        }

        [Fact]
        public async Task UpdateRecipe_ShouldUpdateRecipe()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 2, Name = "Recipe 2" },
                new Recipe { RecipeId = 3, Name = "Recipe 3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);
            mockDbContext.Setup(c => c.Recipes.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] id) => recipes.Single(r => r.RecipeId == (int)id[0]));

            var cmd = new UpdateRecipeCommand { RecipeId = 2, Name = "New name", IsVegan = true };
            var service = new RecipeService(mockDbContext.Object);
            await service.UpdateRecipe(cmd);
            var result = mockDbContext.Object.Recipes.Single(r => r.RecipeId == cmd.RecipeId);

            Assert.NotNull(result);
            Assert.Equal(cmd.RecipeId, result.RecipeId);
            Assert.Equal(cmd.Name, result.Name);
            Assert.Equal(cmd.IsVegan, result.IsVegan);
        }

        #endregion

        #region DeleteRecipe

        [Fact]
        public async Task DeleteRecipe_ShouldThrowExceptionIfRecipeNotFound()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var id = 2;
            var service = new RecipeService(mockDbContext.Object);

            await Assert.ThrowsAsync<RecipeNotFoundException>(async () => await service.DeleteRecipe(id));
        }

        [Fact]
        public async Task DeleteRecipe_ShouldMarkRecipeAsDeleted()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, Name = "Recipe 1" },
                new Recipe { RecipeId = 2, Name = "Recipe 2", IsDeleted = false },
                new Recipe { RecipeId = 3, Name = "Recipe 3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);
            mockDbContext.Setup(c => c.Recipes.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] id) => recipes.Single(r => r.RecipeId == (int)id[0]));

            var id = 2;
            var service = new RecipeService(mockDbContext.Object);
            await service.DeleteRecipe(id);
            var result = mockDbContext.Object.Recipes.Single(r => r.RecipeId == id);

            Assert.NotNull(result);
            Assert.Equal(id, result.RecipeId);
            Assert.True(result.IsDeleted);
        }

        #endregion

        #region GetUserRecipes

        [Fact]
        public async Task GetUserRecipes_ShouldNotReturnRecipesIfThereAreNoOnesForSpecifiedUser()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, CreatedById = "1" },
                new Recipe { RecipeId = 2, CreatedById = "3" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var userId = "2";
            var count = 4;
            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetUserRecipes(userId, count);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserRecipes_ShouldReturnRecipesOnlyForSpecifiedUser()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, CreatedById = "1" },
                new Recipe { RecipeId = 2, CreatedById = "3" },
                new Recipe { RecipeId = 3, CreatedById = "1" }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var userId = "1";
            var count = 4;
            var expected = recipes
                .Where(r => r.CreatedById == userId)
                .ToArray();

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetUserRecipes(userId, count);

            Assert.NotNull(result);
            Assert.Equal(expected.Length, result.Count());

            foreach (var recipe in expected)
            {
                Assert.Contains(result, r => r.Id == recipe.RecipeId);
            }
        }

        [Fact]
        public async Task GetUserRecipes_ShouldReturnOnlyNotDeletedRecipes()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, CreatedById = "1", IsDeleted = false },
                new Recipe { RecipeId = 2, CreatedById = "1", IsDeleted = true },
                new Recipe { RecipeId = 3, CreatedById = "1", IsDeleted = false }
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var userId = "1";
            var count = 4;
            var expected = recipes
                .Where(r => !r.IsDeleted)
                .ToArray();

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetUserRecipes(userId, count);

            Assert.NotNull(result);
            Assert.Equal(expected.Length, result.Count());

            foreach (var recipe in expected)
            {
                Assert.Contains(result, r => r.Id == recipe.RecipeId);
            }
        }

        [Fact]
        public async Task GetUserRecipes_ShouldReturnRecipesInDescendingOrderOfLastModified()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, CreatedById = "1", LastModified = new DateTime(2021, 1, 1) },
                new Recipe { RecipeId = 2, CreatedById = "1", LastModified = new DateTime(2022, 1, 1) },
                new Recipe { RecipeId = 3, CreatedById = "1", LastModified = new DateTime(2020, 1, 1) },
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var userId = "1";
            var count = 4;
            var expected = recipes
                .OrderByDescending(r => r.LastModified)
                .ToArray();

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetUserRecipes(userId, count);

            Assert.NotNull(result);
            Assert.Equal(expected.Length, result.Count());

            foreach (var recipe in expected)
            {
                Assert.Contains(result, r => r.Id == recipe.RecipeId);
            }
        }

        [Theory]
        [InlineData("1", 2, 2)]
        [InlineData("2", 2, 2)]
        [InlineData("3", 2, 1)]
        public async Task GetUserRecipes_ShouldReturnNoMoreRecipesThanRequested(string userId, int count, int expectedCount)
        {
            var recipes = new List<Recipe>
            {
                new Recipe { RecipeId = 1, CreatedById = "1", },
                new Recipe { RecipeId = 2, CreatedById = "1", },
                new Recipe { RecipeId = 3, CreatedById = "1", },
                new Recipe { RecipeId = 4, CreatedById = "2", },
                new Recipe { RecipeId = 5, CreatedById = "2", },
                new Recipe { RecipeId = 7, CreatedById = "3", },
            };
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Recipes).ReturnsDbSet(recipes);

            var service = new RecipeService(mockDbContext.Object);
            var result = await service.GetUserRecipes(userId, count);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        #endregion
    }
}
