using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.ViewModels
{
    public class EditRoleViewModel
    {
        [Required]
		public string roleName { get; set; }
		public string? roleID { get; set; }
		public string[]? addIDs { get; set; }
		public string[]? deleteIDs { get; set; }
		public IdentityRole? role { get; set; }
        public IEnumerable<AppUser>? members { get; set; }
        public IEnumerable<AppUser>? nonMembers { get; set; }
    }
}
