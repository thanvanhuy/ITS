using Microsoft.AspNetCore.Identity;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Interfaces
{
    public interface IIdentityRoleRepository
    {
        Task<IEnumerable<IdentityRole>> GetAllRoles();
        Task<IdentityRole> GetRoleByID(string roleID);
        Task<IEnumerable<AppUser>> GetUsersFromRole(string roleID);
        Task<IEnumerable<IdentityRole>> GetRolesByName(string roleName);
        Task<IEnumerable<IdentityRole>> GetRolesById(string roleID);
        Task<bool> Add(string roleName);
        Task<bool> Delete(IdentityRole role);
    }
}
