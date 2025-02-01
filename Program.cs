using Blog_Zaliczeniowy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog_Zaliczeniowy.Models;

namespace Blog_Zaliczeniowy
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.AddControllersWithViews();

            var app = builder.Build();


			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Posts/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
                name: "default",
                pattern: "{controller=posts}/{action=Index}/{id?}");
            app.MapRazorPages();

			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

				var roles = new[] { "Administrator", "User" };

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
						await roleManager.CreateAsync(new IdentityRole(role));
				}

				var adminEmail = "admin@admin.com";
				var adminPassword = "Admin123#";

				if (await userManager.FindByEmailAsync(adminEmail) == null)
				{
					var adminUser = new ApplicationUser
					{
						UserName = adminEmail,
						Email = adminEmail,
						EmailConfirmed = true,
						Nickname = "Admin"
					};

					var result = await userManager.CreateAsync(adminUser, adminPassword);
					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(adminUser, "Administrator");
					}
					else
					{
						var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
						foreach (var error in result.Errors)
						{
							logger.LogError($"Nie udało się utworzć konta: {error.Description}");
						}
					}
				}
			}


			app.Run();
        }
    }
}
