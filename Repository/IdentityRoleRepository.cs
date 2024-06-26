using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
// Reference: Identity roles: https://www.yogihosting.com/aspnet-core-identity-roles/
namespace VVA.ITS.WebApp.Repository
{
    public class IdentityRoleRepository : IIdentityRoleRepository
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext context;
        public IdentityRoleRepository(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<bool> Add(string roleName)
        {
            IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded) return true;
            return false;
        }

        public async Task<bool> Delete(IdentityRole role)
        {
            IdentityResult result = await roleManager.DeleteAsync(role);
            if (result.Succeeded) return true;
            return false;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            if (this.roleManager == null) { return Enumerable.Empty<IdentityRole>(); }
            return await this.roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetRoleByID(string roleID)
        {
            if (this.roleManager == null) { return null; }
            return await this.roleManager.FindByIdAsync(roleID);
        }

        public async Task<IEnumerable<AppUser>> GetUsersFromRole(string roleID)
        {
            if (this.roleManager == null) { return Enumerable.Empty<AppUser>(); }
            IdentityRole role = await this.GetRoleByID(roleID);
            if (this.userManager == null) { return Enumerable.Empty<AppUser>(); }
            List<AppUser> members = new List<AppUser>();
            foreach (AppUser user in userManager.Users)
            {
                if (await this.userManager.IsInRoleAsync(user, role.Name))
                    members.Add(user);
            }
            return members;
        }

        public async Task<IEnumerable<IdentityRole>> GetRolesById(string roleID)
        {
            //if (roleID.IsNullOrEmpty()) { return Enumerable.Empty<IdentityRole>(); }
            return await this.context.roles
                                      .Where(p => p.Id.Contains(roleID))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<IdentityRole>> GetRolesByName(string roleName)
        {
            //if (roleName.IsNullOrEmpty()) { return Enumerable.Empty<IdentityRole>(); }
            return await this.context.roles
                                      .Where(p => p.Name.Contains(roleName))
                                      .ToListAsync();
        }
    }
}
