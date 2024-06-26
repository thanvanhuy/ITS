using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Interfaces
{
    public interface IDataxe
    {
        Task<IEnumerable<DataXe>> GetAllDataXe();
        Task<List<DataXe>> SeachWeight(SeachVehicles seachVehicles);
        Task<DataXe> SeachWeightById(int id);
        Task<bool> Update(DataXe dataXe);
        
    }
}
