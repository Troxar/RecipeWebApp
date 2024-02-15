using Microsoft.AspNetCore.TestHost;
using RecipeWebApp.Middleware;

namespace RecipeWebApp.Tests
{
    public class HeadersMiddlewareTests
    {
        [Fact]
        public async Task HeadersMiddleware_ShouldSetXContentTypeOptionsHeader()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseMiddleware<HeadersMiddleware>();
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Hello");
                    });
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();
            var actual = response.Headers
                .GetValues("X-Content-Type-Options")
                .FirstOrDefault();
            Assert.Equal("nosniff", actual);
        }

        [Fact]
        public async Task HeadersMiddleware_ShouldCallNextMiddleware()
        {
            var nextCalled = false;
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseMiddleware<HeadersMiddleware>();
                    app.Use(next => context =>
                    {
                        nextCalled = true;
                        return Task.CompletedTask;
                    });
                    app.Run(async context => await context.Response.WriteAsync("Hello"));
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();
            Assert.True(nextCalled);
        }
    }
}
