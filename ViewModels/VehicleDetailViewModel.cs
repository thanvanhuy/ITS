using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VVA.ITS.WebApp.Models;
using AppUtilObjectCore;
using VVA.ITS.WebApp.Services.Helps;

namespace VVA.ITS.WebApp.ViewModels
{
    public class VehicleDetailViewModel
    {
        public int Id { get; set; }
        public string? Device { get; set; }
        public DateTime Time { get; set; }
        public string? Plate { get; set; }
        public string? Type { get; set; }
        public int? Speed { get; set; }
        public string? Direction { get; set; }
        public string? Region { get; set; }
        public float Confidence { get; set; }
        public string? Plate_Color { get; set; }
        public string? Vehicle_Type { get; set; }
        public string? Vehicle_Color { get; set; }
        public string? Vehicle_Brand { get; set; }
        public string? Plate_Image { get; set; }
        public string? Full_Image { get; set; }

        public void GetData(SpeedCAM vehicle)
        {
            this.Id = vehicle.Id;
            this.Device  = "DT874";
            this.Time = vehicle.Time;
            this.Plate = vehicle.Plate;
            this.Type = vehicle.Type;
            this.Speed = vehicle.Speed;
            this.Direction = vehicle.Direction;
            this.Plate_Color = vehicle.Plate_Color;
            this.Vehicle_Type = vehicle.Vehicle_Type;
            this.Vehicle_Color = vehicle.Vehicle_Color;
            this.Vehicle_Brand = vehicle.Vehicle_Brand;
            this.Plate_Image = clsHelps.GetRelativePath(vehicle.Plate_Image);
            this.Full_Image = clsHelps.GetRelativePath(vehicle.Full_Image);
            if (!System.IO.File.Exists(this.Plate_Image))
            {
                this.Plate_Image = clsHelps.imagenotfound;
            }
            if (!System.IO.File.Exists(this.Full_Image))
            {
                this.Full_Image = clsHelps.imagenotfound;
            }
        }
    }
}
