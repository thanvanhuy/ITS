using Microsoft.EntityFrameworkCore;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Repository
{
    public class DataxeReponsitory:IDataxe
    {
        private readonly ApplicationDbContext context;
        public DataxeReponsitory(ApplicationDbContext context)
        {
            this.context = context;   
        }
        public async Task<IEnumerable<DataXe>> GetAllDataXe()
        {
            return await this.context.tbl_Data_Xe.OrderByDescending(n=>n.Thoigian).ToListAsync();
        }
        public async Task<List<DataXe>> SeachWeight(SeachVehicles seachVehicles)
        {
            string sql = "select * from tbl_Data_Xe where Thoigian>'" + seachVehicles.starttime + "' and Thoigian<'" + seachVehicles.endtime + "'";
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
                sql += " and TRY_CAST(Quataitong AS float) > 0 ";
            }
            if (seachVehicles.TypeViolation == 2)
            {
                sql += " and TRY_CAST(NULLIF(REPLACE(Quataitheogp, '%', ''), '') AS float) > 0 ";
            }
            return await context.tbl_Data_Xe.FromSqlRaw(sql).ToListAsync();
        }
        public async Task<DataXe> SeachWeightById(int id)
        {
            return await context.tbl_Data_Xe.FindAsync(id);
        }
        public async Task<bool> Update(DataXe dataXe)
        {
            this.context.Update(dataXe);
            var saved = await this.context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
