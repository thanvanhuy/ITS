using TableDependency.SqlClient;
using VVA.ITS.WebApp.Hubs;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.Interfaces;
using AppUtilObjectCore;
using TableDependency.SqlClient.Base;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.InkML;

namespace VVA.ITS.WebApp.SubscribeTableDependencies
{
    public class SubscribeVehicleTableDependency : ISubscribeTableDependency
    {
        private DashboardHub dashboardHub;
        private SqlTableDependency<SpeedCAM> tableDependency;
        private Logger logger;
        public  SubscribeVehicleTableDependency(DashboardHub dashboardHub)
        {
            this.dashboardHub = dashboardHub;
        }
        public void SubscribeTableDependency(string connectionString)
        {
            var mapper = new ModelToTableMapper<SpeedCAM>();
            mapper.AddMapping(c => c.Id, "id");
            mapper.AddMapping(c => c.Device, "device");
            mapper.AddMapping(c => c.Time, "time");
            mapper.AddMapping(c => c.Type, "type");
            mapper.AddMapping(c => c.Speed, "speed");
            mapper.AddMapping(c => c.Direction, "direction");
            mapper.AddMapping(c => c.Vehicle_Type, "vehicle_type");
            mapper.AddMapping(c => c.Vehicle_Color, "vehicle_color");
            mapper.AddMapping(c => c.Vehicle_Brand, "vehicle_brand");
            mapper.AddMapping(c => c.Plate, "plate");
            mapper.AddMapping(c => c.Plate_Color, "plate_color");
            mapper.AddMapping(c => c.Plate_Image, "plate_image");
            mapper.AddMapping(c => c.Full_Image, "full_image");

         this.tableDependency = new SqlTableDependency<SpeedCAM>(connectionString, "Speed_CAM", mapper: mapper);
            this.tableDependency.OnChanged += VehicleTableDependency_OnChanged;
            this.tableDependency.OnError += VehicleTableDependency_OnError;
            this.tableDependency.Start();
        }

        private void VehicleTableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            if (logger == null) logger = new Logger();
            logger.Write("VehicleTableDependency_OnError", nameof(SpeedCAM) + " SqlTableDependency error: " + e.Error.Message, Logger.LogType.ERROR);
        }
        private async void VehicleTableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<SpeedCAM> e)
        {

            //Console.WriteLine("platesend " + DashboardHub.platesend);
            //Console.WriteLine("speedsend " + DashboardHub.speedsend);
            //Console.WriteLine("directionsend " + DashboardHub.directionsend);
           
            if (!DashboardHub.checkfind)
            {
                await dashboardHub.SendVehicles();
            }
        }
    }
}
