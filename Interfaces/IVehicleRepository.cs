using System.Collections;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Speed_CAM_NEW>> GetAllVehicles();
        Task<IEnumerable<Speed_CAM_NEW>> SeachAllVehicles(Seach seach);
        Task<IEnumerable<Speed_CAM_NEW>> GetAllVehiclesxemayvipham();
        Task<IEnumerable<Speed_CAM_NEW>> GetAllVehiclesoto();
        Task<IEnumerable<Speed_CAM_NEW>> GetAllVehiclesotovipham();
        Task<Speed_CAM_NEW?> GetVehicle(int? vehicleID);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByLocation(int locationID);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByTime(string fromTime, string toTime);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByType(string type);
        Task<IEnumerable<Speed_CAM_NEW>> GetAllVehicleByType(int type);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesBySpeed(int? fromSpeed, int? toSpeed);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByDirection(string direction);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByVehicleType(string vehicleType);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByVehicleColor(string vehicleColor);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByVehicleBrand(string vehicleBrand);
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByPlate(string plateNumber);        
        Task<IEnumerable<Speed_CAM_NEW>> GetVehiclesByPlateColor(string plateColor);        
        Task<IEnumerable<SpeedCAM>> GetTopVehicles(int number, bool asc = true);
        Task<IEnumerable<SpeedCAM>> GetTopVehiclesAdmin(int number, bool asc = true);
        Task<IEnumerable<VehicleTypeCount>> GetVehiclesType();
        //Task<IEnumerable<SpeedCAM>> GetTopVehiclesByLocationNames(string[] locationNames, int number, bool asc = true);
        Task<bool> VehicleExists(int vehicleID);
        Task<bool> Add(Speed_CAM_NEW vehicle);
        Task<bool> Update(Speed_CAM_NEW vehicle);
        Task<bool> Delete(Speed_CAM_NEW vehicle);
        Task<bool> Save();
    }
}
