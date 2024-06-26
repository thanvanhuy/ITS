using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<SpeedCAM>> GetAllVehicles();
        Task<IEnumerable<SpeedCAM>> SeachAllVehicles(Seach seach);
        Task<IEnumerable<SpeedCAM>> GetAllVehiclesxemayvipham();
        Task<IEnumerable<SpeedCAM>> GetAllVehiclesoto();
        Task<IEnumerable<SpeedCAM>> GetAllVehiclesotovipham();
        Task<SpeedCAM?> GetVehicle(int? vehicleID);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByLocation(int locationID);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByTime(string fromTime, string toTime);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByType(string type);
        Task<IEnumerable<SpeedCAM>> GetAllVehicleByType(int type);
        Task<IEnumerable<SpeedCAM>> GetVehiclesBySpeed(int? fromSpeed, int? toSpeed);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByDirection(string direction);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByVehicleType(string vehicleType);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByVehicleColor(string vehicleColor);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByVehicleBrand(string vehicleBrand);
        Task<IEnumerable<SpeedCAM>> GetVehiclesByPlate(string plateNumber);        
        Task<IEnumerable<SpeedCAM>> GetVehiclesByPlateColor(string plateColor);        
        Task<IEnumerable<SpeedCAM>> GetTopVehicles(int number, bool asc = true);
        //Task<IEnumerable<SpeedCAM>> GetTopVehiclesByLocationNames(string[] locationNames, int number, bool asc = true);
        Task<bool> VehicleExists(int vehicleID);
        Task<bool> Add(SpeedCAM vehicle);
        Task<bool> Update(SpeedCAM vehicle);
        Task<bool> Delete(SpeedCAM vehicle);
        Task<bool> Save();
    }
}
