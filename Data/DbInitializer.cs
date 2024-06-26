using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using VVA.ITS.WebApp.Models;

// Reference: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-7.0
namespace VVA.ITS.WebApp.Data
{
	public static class DbInitializer
	{
		public static async void InitializeRole(RoleManager<IdentityRole> roleManager)
		{
			if (!await roleManager.RoleExistsAsync("admin"))
				await roleManager.CreateAsync(new IdentityRole("admin"));
			if (!await roleManager.RoleExistsAsync("user"))
				await roleManager.CreateAsync(new IdentityRole("user"));
		}

		public static async void InitializeUser(UserManager<AppUser> userManager)
		{
			var hieutd = await userManager.FindByNameAsync("hieutd");
			if (hieutd == null)
			{
				hieutd = new AppUser()
				{
					UserName = "hieutd",
					Email = "hieutd@vvaits.vn",
					EmailConfirmed = true
				};
				await userManager.CreateAsync(hieutd, "W25axsz@$");
				await userManager.CreateAsync(hieutd, "admin");
			}
		}
		public static async void InitializeData(ApplicationDbContext context)
		{

			context.Database.EnsureCreated();
		}

	}
}
