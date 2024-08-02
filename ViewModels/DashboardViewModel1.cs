using VVA.ITS.WebApp.Models;
using X.PagedList;

namespace VVA.ITS.WebApp.ViewModels
{
    public class DashboardViewModel1
    {
        public IPagedList<VehicleDetailViewModel> Vehicles { get; set; }
        public Seach? Search { get; set; }
    }
}
