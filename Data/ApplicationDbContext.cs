using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }
        public DbSet<AppUser> users { get; set; }
        public DbSet<IdentityRole> roles { get; set; }
        public DbSet<SpeedCAM> Speed_CAM { get; set; }
        public DbSet<DataXe> tbl_Data_Xe { get; set; }

    }
}
