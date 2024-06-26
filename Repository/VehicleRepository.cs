using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext context;
        public VehicleRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> Add(SpeedCAM vehicle)
        {
            await this.context.AddAsync(vehicle);
            return await Save();
        }

        public async Task<bool> Delete(SpeedCAM vehicle)
        {
            this.context.Remove(vehicle);
            return await Save();
        }

        public async Task<IEnumerable<SpeedCAM>> GetAllVehicles()
        {
            return await this.context.Speed_CAM.Where(n=>n.Time> DateTime.Now.AddDays(-7) && n.Vehicle_Type== "Motorbike").ToListAsync();
        }
        public async Task<IEnumerable<SpeedCAM>> GetAllVehiclesxemayvipham()
        {
            string sql = " SELECT  * FROM Speed_CAM WHERE Time > DATEADD(DAY, -7, GETDATE()) and speed > 45 and vehicle_type ='Motorbike'";
            return await this.context.Speed_CAM
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }
        public async Task<IEnumerable<SpeedCAM>> SeachAllVehicles(Seach seach)
        {
            string sql = "select * from Speed_CAM where time>'" + seach.starttime + "' and time<'" + seach.endtime + "'";
            if(seach.platesend.Length>5)
            {
                sql += " and Plate= '"+seach.platesend+"'";
            }
            if (seach.VehicleType == 1)
            {
                sql += " and vehicle_type = 'Motorbike'";

            }
            if (seach.VehicleType == 2)
            {
                sql += " and vehicle_type != 'Motorbike'";

            }
            if (seach.directionsend==1)
            {
                sql += " and Direction= 'Away'";
            }
            if (seach.directionsend == 2)
            {
                sql += " and Direction= 'Approach'";
            }
            if (seach.speedsend == 1)
            {
                sql += " and speed < 55 ";
            }
            if (seach.speedsend == 2)
            {
                sql += " and speed >= 55 and speed < 60 ";
            }
            if (seach.speedsend == 3)
            {
                sql += " and speed >= 60 and speed < 70 ";
            }
            if (seach.speedsend == 4)
            {
                sql += " and speed >= 70 and speed < 80 ";
            }
            if (seach.speedsend == 5)
            {
                sql += " and speed >= 80 ";
            }
            return await this.context.Speed_CAM
                 .FromSqlRaw(sql)
                 .ToListAsync();

        }
        public async Task<IEnumerable<SpeedCAM>> GetAllVehiclesoto()
        {
            return await this.context.Speed_CAM.Where(n => n.Time > DateTime.Now.AddDays(-7) && n.Vehicle_Type != "Motorbike").ToListAsync();
        }
        public async Task<IEnumerable<SpeedCAM>> GetAllVehiclesotovipham()
        {
            string sql = " SELECT * FROM Speed_CAM WHERE Time > DATEADD(DAY, -7, GETDATE()) and  speed  > 45 and vehicle_type !='Motorbike'";
            return await this.context.Speed_CAM
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }
        public async Task<IEnumerable<SpeedCAM>> GetAllVehicleByType(int type)
        {
            string sql = "";
            if (type == 2)
            {
                sql=  "SELECT top 50 * FROM Speed_CAM WHERE vehicle_type !='Motorbike'";
            }
            else
            {
                sql = "SELECT top 50 * FROM Speed_CAM WHERE vehicle_type ='Motorbike'";
            }
             
            return await this.context.Speed_CAM
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }
        public async Task<IEnumerable<SpeedCAM>> GetTopVehicles(int number, bool asc = true)
        {
            IQueryable<SpeedCAM> query = this.context.Speed_CAM;

            // Sắp xếp dữ liệu theo thứ tự tăng dần hoặc giảm dần theo Id
            query = asc ? query.OrderBy(v => v.Id) : query.OrderByDescending(v => v.Id);

            
            var sqlQuery = query.Take(number).ToQueryString();
            try
            {
                return await query.Take(number).ToListAsync();
            }catch (Exception ex) { }

            return null;

        }


        //public async Task<IEnumerable<SpeedCAM>> GetTopVehiclesByLocationNames(string[] locationNames, int number, bool asc = true)
        //{
        //    List<int> locationIDs = new List<int>();
        //    for (int i=0; i< locationNames.Length; i++)
        //    {
        //        var location = await this.context.Speed_CAM.FirstOrDefaultAsync(e => e.Device != null && e.Device.Equals(locationNames[i]));
        //        if (location != null) locationIDs.Add(location.id);
        //    }
        //    string condition = string.Empty;
        //    foreach (int locationID in locationIDs)
        //    {
        //        if (condition != string.Empty) condition += " OR ";
        //        condition += "locationID = " + locationID;
        //    }

        //    if (asc) return await this.context.Speed_CAM
        //                    .FromSqlRaw("select top " + number + " * from Speed_CAM where " + condition)
        //                    .ToListAsync();
        //    else return await this.context.Speed_CAM
        //                    .FromSqlRaw("select top "+number+ " * from Speed_CAM where " + condition+" order by id desc")
        //                    .ToListAsync();
        //}

        public async Task<SpeedCAM?> GetVehicle(int? vehicleID)
        {
            return await this.context.Speed_CAM.FirstOrDefaultAsync(e => e.Id == vehicleID);
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByDirection(string direction)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Direction != null && p.Direction.Contains(direction)).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByLocation(int locationID)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Id == locationID)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByPlate(string plateNumber)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Plate != null && p.Plate.Contains(plateNumber))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByPlateColor(string plateColor)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Plate_Color != null && p.Plate_Color.Contains(plateColor))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesBySpeed(int? fromSpeed, int? toSpeed)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            if (fromSpeed == null) return await this.context.Speed_CAM
                                      .Where(p => p.Speed != null && (p.Speed) <= toSpeed).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
            else if (toSpeed == null) return await this.context.Speed_CAM
                                      .Where(p => p.Speed != null && (p.Speed) >= fromSpeed).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
            else return await this.context.Speed_CAM
                                      .Where(p => p.Speed != null && (p.Speed) >= fromSpeed && (p.Speed) <= toSpeed).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByTime(string fromTime, string toTime)
        {
            string sql = "select * from Speed_CAM where time>'"+fromTime+ "' and time<'" + toTime + "'";
            return await this.context.Speed_CAM
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByType(string type)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Type != null && p.Type.Contains(type))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByVehicleBrand(string vehicleBrand)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Vehicle_Brand != null && p.Vehicle_Brand.Contains(vehicleBrand))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByVehicleColor(string vehicleColor)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Vehicle_Color != null && p.Vehicle_Color.Contains(vehicleColor))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<SpeedCAM>> GetVehiclesByVehicleType(string vehicleType)
        {
            if (this.context == null) return Enumerable.Empty<SpeedCAM>();
            else if (this.context.Speed_CAM == null) return Enumerable.Empty<SpeedCAM>();
            return await this.context.Speed_CAM
                                      .Where(p => p.Vehicle_Type != null && p.Vehicle_Type.Contains(vehicleType))
                                      .ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await this.context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> Update(SpeedCAM vehicle)
        {
            this.context.Update(vehicle);
            return await Save();
        }

        public async Task<bool> VehicleExists(int vehicleID)
        {
            return await this.context.Speed_CAM.AnyAsync(e => e.Id == vehicleID);
        }
    }
}
