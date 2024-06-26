using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Net;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                if (!context.users.Any())
                {
                    //context.users.AddRange(new List<User>()
                    //{
                    //    new User()
                    //    {
                    //        UserName = "admin",
                    //        PasswordHash = "$2y$10$teXE/prPSzphDafApyD5UO4QAvTeMZRbJ9wNm0U4n6k0ytzn9fiky"
                    //    },
                    //    new User()
                    //    {
                    //        UserName = "user",
                    //        PasswordHash = "$2y$10$teXE/prPSzphDafApyD5UO4QAvTeMZRbJ9wNm0U4n6k0ytzn9fiky"
                    //    }
                    //});
                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            //using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            //{
            //    //Roles
            //    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //    if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            //        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            //    if (!await roleManager.RoleExistsAsync(UserRoles.User))
            //        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            //    //Users
            //    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            //    var adminUser = await userManager.FindByNameAsync("admin");
            //    if (adminUser == null)
            //    {
            //        var newAdminUser = new AppUser()
            //        {
            //            UserName = "admin",
            //            Email = "tranduchieu@gmail.com",
            //            EmailConfirmed = true,
            //        };
            //        await userManager.CreateAsync(newAdminUser, "Vva@123");
            //        await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
            //    }

            //    var appUser = await userManager.FindByNameAsync("user");
            //    if (appUser == null)
            //    {
            //        var newAppUser = new AppUser()
            //        {
            //            UserName = "user",
            //            Email = "user@gmail.com",
            //            EmailConfirmed = true,
            //        };
            //        await userManager.CreateAsync(newAppUser, "Vva@123");
            //        await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
            //    }
            //}
        }
    }
}
