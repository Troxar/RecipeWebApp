using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecipeWebApp.Authorization;
using RecipeWebApp.Configs;
using RecipeWebApp.Entities;
using RecipeWebApp.Infrastructure;
using RecipeWebApp.Middleware;
using RecipeWebApp.Services;

namespace RecipeWebApp
{
    public class Startup
    {
        IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddSwaggerGen();

            services.Configure<RecipeApiConfig>(_configuration.GetSection(nameof(RecipeApiConfig)));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanManageRecipe", policyBuilder =>
                    policyBuilder.AddRequirements(new IsRecipeOwnerRequirement()));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSecurityHeaders();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
