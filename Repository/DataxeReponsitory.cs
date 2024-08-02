using BoldReports.Processing.ObjectModels;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Repository
{
    public class DataxeReponsitory:IDataxe
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        public DataxeReponsitory(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        private async Task<int> typeuser()
        {
            var currentUser = httpContextAccessor.HttpContext.User;
            string IdUser = currentUser.Identity.Name;
            // dữ liệu của tầm nhìn
            if (IdUser == "svtt")
            {
                return 1;
            }
            // dữ liệu của khách
            if (IdUser == "svtt1")
            {
                return 2;
            }
            //xem hết admin
            return 3;
        }
        public async Task<IEnumerable<DataXe>> GetAllDataXe()
        {
            int IdUser = await typeuser();
            if (IdUser == 1)
            {
                return await this.context.tbl_Data_Xe.Where(n=>n.IdUser==1).OrderByDescending(n => n.Thoigian).ToListAsync();
            }else if (IdUser == 2)
            {
                return await this.context.tbl_Data_Xe.Where(n => n.IdUser == 2).OrderByDescending(n => n.Thoigian).ToListAsync();
            }
            return await this.context.tbl_Data_Xe.OrderByDescending(n => n.Thoigian).ToListAsync();
        }
        public async Task<List<DataXe>> SeachWeight(SeachVehicles seachVehicles)
        {
            string sql = "select * from tbl_Data_Xe where Thoigian>'" + seachVehicles.starttime + "' and Thoigian<'" + seachVehicles.endtime + "'";
            int IdUser = await typeuser();
            if (IdUser == 1)
            {
                sql += "and IdUser=1 ";
            }
            if (IdUser == 2)
            {
                sql += "and IdUser=2 ";
            }
            if (seachVehicles.Plate.Length > 5)
            {
                sql += " and (Biensotruoc= '" + seachVehicles.Plate + "' or Biensosau='" + seachVehicles.Plate + "')";
            }
            if (seachVehicles.VehicleType == 1)
            {
                sql += " and kieuxe<7 ";
            }
            if (seachVehicles.VehicleType == 2)
            {
                sql += " and kieuxe>6 ";
            }
            if (seachVehicles.TypeViolation==1)
            {
                sql += " and Quataitong  > 0 ";
                // sql += " and TRY_CAST(Quataitong AS float) > 0 ";
            }
            if (seachVehicles.TypeViolation == 2)
            {
                sql += " and Quataitheogp  > 0 ";
                //sql += " and TRY_CAST(NULLIF(REPLACE(Quataitheogp, '%', ''), '') AS float) > 0 ";
            }
            return await context.tbl_Data_Xe.FromSqlRaw(sql).ToListAsync();
        }
        public async Task<DataXe> SeachWeightById(string id)
        {
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                return null;
            }
            var dataXe = await context.tbl_Data_Xe
                                      .FirstOrDefaultAsync(n => n.Id == guid);
            return dataXe;
        }

        public async Task<bool> Update(DataXe dataXe)
        {
            this.context.Update(dataXe);
            var saved = await this.context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
