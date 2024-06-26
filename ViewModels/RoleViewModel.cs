using Microsoft.AspNetCore.Identity;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.Services;

namespace VVA.ITS.WebApp.ViewModels
{
    public class RoleViewModel
    {
        public PaginatedList<IdentityRole> roles { get; set; }
        public UserManager<AppUser> userManager { get; set; }
    }
}
