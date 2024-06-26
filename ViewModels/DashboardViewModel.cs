using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.ViewModels
{
    public class DashboardViewModel
    {
        public string? hostAddress { get; set; }
        public string? hostPort { get; set; }
        public IEnumerable<VehicleDetailViewModel>? vehicles { get; set; }
       // public IEnumerable<Location>? locations { get; set; }
    }
}
