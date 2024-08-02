using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.Services.Helps;

namespace VVA.ITS.WebApp.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        public VehicleRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        private async Task<int> typeuser()
        {
            try
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
            }catch (Exception) { return 4; }
            
        }
        public async Task<bool> Add(Speed_CAM_NEW vehicle)
        {
            await this.context.AddAsync(vehicle);
            return await Save();
        }

        public async Task<bool> Delete(Speed_CAM_NEW vehicle)
        {
            this.context.Remove(vehicle);
            return await Save();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetAllVehicles()
        {
            return await this.context.Speed_CAM_NEW.Where(n=>n.Time> DateTime.Now.AddDays(-7) && n.Vehicle_Type== "Motorbike").ToListAsync();
        }
        public async Task<IEnumerable<Speed_CAM_NEW>> GetAllVehiclesxemayvipham()
        {
            string sql = " SELECT  * FROM Speed_CAM_NEW WHERE Time > DATEADD(DAY, -7, GETDATE()) and speed > 45 and vehicle_type ='Motorbike'";
            return await this.context.Speed_CAM_NEW
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }
        public async Task<IEnumerable<Speed_CAM_NEW>> SeachAllVehicles(Seach seach)
        {
            // truyền int vào xử lý tránh lỗi người dùng 
            if (seach.platesend.Length>12)
            {
                return null;
            }
            string sql = "select * from Speed_CAM_NEW where time>'" + seach.starttime + "' and time<'" + seach.endtime + "'";
            if(seach.platesend.Length>5)
            {
                sql += " and Plate= '"+seach.platesend+"'";
            }
            if (seach.VehicleType >0)
            {
                string vehicletype = clsHelps.getVehicleTypeCondition(seach.VehicleType);
                sql +=  vehicletype ;

            }
            if(seach.directionsend > 0)
            {
                string device = clsHelps.getDeviceType(seach.directionsend);
                int idUser = await typeuser().ConfigureAwait(false);
                if (idUser==1)
                {
                    if (seach.directionsend==5)
                    {
                        sql += " and device != '237PVC'";
                    }
                    else
                    {
                        sql += " and device= '" + device + "'";
                    }
                }
                else
                {
                    sql += " and device= '" + device + "'";
                }
            }
            else
            {
                sql += " and device != '237PVC'";
            }
            if (seach.speedsend > 0)
            {
                string speed = clsHelps.getSpeedCondition(seach.speedsend);
                sql += speed;
            }
            if (seach.platecolor > 0)
            {
                string platecolor = clsHelps.getPlateColor(seach.platecolor);
                sql += platecolor;
            }
            if (seach.vehiclecolor > 0)
            {
                string vehiclecolor = clsHelps.getVeheicleColor(seach.vehiclecolor);
                sql += vehiclecolor;
            }
            if (seach.vehiclebrand > 0)
            {
                string vehiclebrand = clsHelps.getVehicleBrand(seach.vehiclebrand);
                sql += vehiclebrand;
            }

            return await this.context.Speed_CAM_NEW
                 .FromSqlRaw(sql)
                 .ToListAsync();

        }
        public async Task<IEnumerable<Speed_CAM_NEW>> GetAllVehiclesoto()
        {
            return await this.context.Speed_CAM_NEW.Where(n => n.Time > DateTime.Now.AddDays(-7) && n.Vehicle_Type != "Motorbike").ToListAsync();
        }
        public async Task<IEnumerable<Speed_CAM_NEW>> GetAllVehiclesotovipham()
        {
            string sql = " SELECT * FROM Speed_CAM_NEW WHERE Time > DATEADD(DAY, -7, GETDATE()) and  speed  > 45 and vehicle_type !='Motorbike'";
            return await this.context.Speed_CAM_NEW
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }
        public async Task<IEnumerable<Speed_CAM_NEW>> GetAllVehicleByType(int type)
        {
            string sql = "";
            if (type == 2)
            {
                sql= "SELECT top 50 * FROM Speed_CAM_NEW WHERE vehicle_type !='Motorbike'";
            }
            else
            {
                sql = "SELECT top 50 * FROM Speed_CAM_NEW WHERE vehicle_type ='Motorbike'";
            }
             
            return await this.context.Speed_CAM_NEW
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }
        public async Task<IEnumerable<SpeedCAM>> GetTopVehicles(int number, bool asc = true)
        {
            try
            {
               
                IQueryable<SpeedCAM> query = this.context.Speed_CAM;

                
                query = query.Where(n => n.Device != "237PVC");

              
                query = asc ? query.OrderBy(v => v.Id) : query.OrderByDescending(v => v.Id);

                // Take the specified number of records and execute the query asynchronously
                return await query.Take(number).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
              
                return Enumerable.Empty<SpeedCAM>();
            }
        }

        public async Task<IEnumerable<SpeedCAM>> GetTopVehiclesAdmin(int number, bool asc = true)
        {
            try
            {

                IQueryable<SpeedCAM> query = this.context.Speed_CAM;

                query = asc ? query.OrderBy(v => v.Id) : query.OrderByDescending(v => v.Id);

                // Take the specified number of records and execute the query asynchronously
                return await query.Take(number).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {

                return Enumerable.Empty<SpeedCAM>();
            }
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

        public async Task<Speed_CAM_NEW?> GetVehicle(int? vehicleID)
        {
            return await this.context.Speed_CAM_NEW.FirstOrDefaultAsync(e => e.Id == vehicleID);
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByDirection(string direction)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Direction != null && p.Direction.Contains(direction)).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByLocation(int locationID)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Id == locationID)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByPlate(string plateNumber)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Plate != null && p.Plate.Contains(plateNumber))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByPlateColor(string plateColor)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Plate_Color != null && p.Plate_Color.Contains(plateColor))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesBySpeed(int? fromSpeed, int? toSpeed)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            if (fromSpeed == null) return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Speed != null && (p.Speed) <= toSpeed).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
            else if (toSpeed == null) return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Speed != null && (p.Speed) >= fromSpeed).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
            else return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Speed != null && (p.Speed) >= fromSpeed && (p.Speed) <= toSpeed).OrderByDescending(p => p.Id).Take(30)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByTime(string fromTime, string toTime)
        {
            string sql = "select * from Speed_CAM_NEW where time>'" + fromTime+ "' and time<'" + toTime + "'";
            return await this.context.Speed_CAM_NEW
                 .FromSqlRaw(sql)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByType(string type)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Type != null && p.Type.Contains(type))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByVehicleBrand(string vehicleBrand)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Vehicle_Brand != null && p.Vehicle_Brand.Contains(vehicleBrand))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByVehicleColor(string vehicleColor)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Vehicle_Color != null && p.Vehicle_Color.Contains(vehicleColor))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByVehicleType(string vehicleType)
        {
            if (this.context == null) return Enumerable.Empty<Speed_CAM_NEW>();
            else if (this.context.Speed_CAM_NEW == null) return Enumerable.Empty<Speed_CAM_NEW>();
            return await this.context.Speed_CAM_NEW
                                      .Where(p => p.Vehicle_Type != null && p.Vehicle_Type.Contains(vehicleType))
                                      .ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await this.context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> Update(Speed_CAM_NEW vehicle)
        {
            this.context.Update(vehicle);
            return await Save();
        }

        public async Task<bool> VehicleExists(int vehicleID)
        {
            return await this.context.Speed_CAM.AnyAsync(e => e.Id == vehicleID);
        }
        public async Task<IEnumerable<VehicleTypeCount>> GetVehiclesType()
        {
            var vehicleTypeCounts = new List<VehicleTypeCount>();

            // Define the SQL query
            string query = @"
            DECLARE @StartDate DATETIME = DATEADD(DAY, -7, CAST(GETDATE() AS DATE));
            DECLARE @EndDate DATETIME = DATEADD(DAY, 0, CAST(GETDATE() AS DATE));

            SELECT 
                CAST([time] AS DATE) AS [Date],
                CASE 
                    WHEN vehicle_type IN ('-', 'Other') THEN 'Other/Unknown'
                    WHEN vehicle_type IN ('Ambulance', 'Car') THEN 'Car/Other'
                    ELSE vehicle_type
                END AS vehicle_type_combined,
                COUNT(*) AS vehicle_count
            FROM 
                [ITS_DONGTHAP].[dbo].[Speed_CAM_NEW]
            WHERE 
                [time] >= @StartDate
                AND [time] < @EndDate
            GROUP BY 
                CAST([time] AS DATE),
                CASE 
                    WHEN vehicle_type IN ('-', 'Other') THEN 'Other/Unknown'
                    WHEN vehicle_type IN ('Ambulance', 'Car') THEN 'Car/Other'
                    ELSE vehicle_type
                END
            ORDER BY 
                [Date], 
                vehicle_type_combined;
        ";

            // Open connection and execute query
            using (var connection = new SqlConnection("Data Source=192.188.0.10; Initial Catalog=ITS_DONGTHAP; User ID=sa; Password=VVASQL$%^47;MultipleActiveResultSets=True;Connection Timeout=120; TrustServerCertificate=True"))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        vehicleTypeCounts.Add(new VehicleTypeCount
                        {
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            VehicleTypeCombined = reader.GetString(reader.GetOrdinal("vehicle_type_combined")),
                            VehicleCount = reader.GetInt32(reader.GetOrdinal("vehicle_count"))
                        });
                    }
                }
            }

            return vehicleTypeCounts;
        }
    }
}
