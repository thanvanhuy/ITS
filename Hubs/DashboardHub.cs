using Microsoft.AspNetCore.SignalR;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.Repository;
using VVA.ITS.WebApp.ViewModels;
using AppUtilObjectCore;
using System.Diagnostics;
using VVA.ITS.WebApp.Services.Helps;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http.Features;

namespace VVA.ITS.WebApp.Hubs
{
    public  class DashboardHub : Hub
    {
        private IHubContext<DashboardHub> hubContext;
        private IVehicleRepository vehicleRepository;
        private readonly IWebHostEnvironment env;
        private Logger logger;
        public static int speedsend { get; set; }
        public static int directionsend { get; set; }
        public static string platesend { get; set; } = string.Empty;

        public static bool findtime { get; set; } = false;
        public static int VehicleType { get; set; }

        public static bool checkfind { get; set; } = false;
        public  DashboardHub(IHubContext<DashboardHub> hubContext,
                                            IVehicleRepository vehicleRepository, 
                                            IWebHostEnvironment env)
        {
            this.vehicleRepository = vehicleRepository;
            this.hubContext = hubContext;
            speedsend = 0;
            directionsend = 0;
            VehicleType = 0;
            this.env = env;
            platesend = string.Empty;
            this.logger = new Logger();
        }
        public override async Task OnConnectedAsync()
        {
            //var userName = Context.User.Identity.Name;
            //Debug.WriteLine(userName);
            await base.OnConnectedAsync();
        }
        public async Task SendVehicles()
        {
            List<VehicleDetailViewModel> vehicles = await this.GetTopVehicle(20);
            if (vehicles == null) return;
            if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
        }
        public async Task SendVehicles1(Seach seach)
        {
            checkfind=true;
            List<VehicleDetailViewModel> vehicles = await this.Seachvehicles(seach);
            if (vehicles == null) return;
            if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
        }
        public async Task<List<VehicleDetailViewModel>> Seachvehicles(Seach seach)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            if (seach == null) return result;
            var topVehicles = await this.vehicleRepository.SeachAllVehicles(seach);
            foreach (SpeedCAM vehicle in topVehicles)
            {

                string base64PlateImage = string.Empty;

                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                if (!System.IO.File.Exists(vehicle.Plate_Image))
                {
                    base64PlateImage = clsHelps.imagenotfound;
                }
                if (!System.IO.File.Exists(vehicle.Full_Image))
                {
                    base64FullImage = clsHelps.imagenotfound;
                }

                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = vehicle.Device,
                    Time = vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString() == "Away" ? "DT854" : "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }
        public async Task FilterVehicleByvehicleType(int vehicleType)
        {
            VehicleType = vehicleType;
            if (vehicleType == 0)
            {
                await SendVehicles();
            }
            else
            {
                List<VehicleDetailViewModel> vehicles = await this.GetTopVehicleByVehiclesType(vehicleType);
                if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
            }

        }
        public async Task FilterVehicleByDirection(int direction)
        {
            directionsend = direction;
            if (direction == 0)
            {
                await SendVehicles();
            }
            else
            {
                List<VehicleDetailViewModel> vehicles = await this.GetTopVehicleByDirection(direction);
                if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
            }

        }
        public async Task FilterVehicleBySpeed(int speed)
        {
            speedsend = speed;
            if (speed == 0)
            {
                await SendVehicles();
            }
            else
            {
                List<VehicleDetailViewModel> vehicles = await this.GetTopVehicleBySpeed(speed);
                if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
            }
           
        }
        public async Task FilterVehicleByPlate(string plate)
        {
            platesend = plate;
            if (plate.Length == 0) {
                platesend = string.Empty;
                await SendVehicles();
            } 
            else
            {
                Debug.WriteLine(platesend);
                List<VehicleDetailViewModel> vehicles = await this.GetTopVehicleByPlate(plate);
                if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
            }
        }
        public async Task FilterVehicleByDate(string starttime, string endtime)
        {
            {
                findtime = true;
                List<VehicleDetailViewModel> vehicles = await this.GetTopVehicleByDateTime(starttime,endtime);
                if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
            }
        }
        public async Task FilterVehicle(int type,string plate,int derection,DateTime timestart,DateTime timeend,int speed)
        {
            if (plate.Length == 0) await SendVehicles();
            else
            {
                platesend = plate;

                List<VehicleDetailViewModel> vehicles = new List<VehicleDetailViewModel>();

                if (this.hubContext.Clients != null) await this.hubContext.Clients.All.SendAsync("ReceivedVehicles", vehicles);
            }
        }
        public async Task<List<VehicleDetailViewModel>> GetTopVehicle(int number)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            var topVehicles = await this.vehicleRepository.GetTopVehicles(number, false);
            if (topVehicles == null)return null;
            foreach (SpeedCAM vehicle in topVehicles)
            {
                //var location = await this.locationRepository.GetLocation(vehicle.Id);
                // rawDatum = await this.rawDataRepository.GetRawDataByVehicleID(vehicle.Id);
               
                string base64PlateImage = string.Empty;
                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                if (!System.IO.File.Exists(vehicle.Plate_Image))
                {
                    base64PlateImage = clsHelps.imagenotfound;
                }
                if (!System.IO.File.Exists(vehicle.Full_Image))
                {
                    base64FullImage = clsHelps.imagenotfound;
                }
                //if (File.Exists(vehicle.Plate_Image)) base64PlateImage = Utility.fileToBase64(vehicle.Plate_Image);
                //else
                //{
                //    string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "loading_plate_image.png");
                //    //Debug.Write(filePath);
                //    base64PlateImage = Utility.fileToBase64(filePath);
                //}
                //logger.Write("GetTopVehicle", "File " + vehicle.Full_Image + " not found", Logger.LogType.ERROR);


                //if (File.Exists(vehicle.Full_Image)) base64FullImage = Utility.fileToBase64(vehicle.Full_Image);
                //else
                //{
                //    string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "loading_plate_image.png");
                //    base64FullImage = Utility.fileToBase64(filePath);
                //}
                //logger.Write("GetTopVehicle", "File " + vehicle.Region + " not found", Logger.LogType.ERROR);

                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = "DT854",
                    Time =vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString()== "Away" ? "DT854": "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }

       
        public async Task<List<VehicleDetailViewModel>> GetTopVehicleByPlate(string plate)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            var topVehicles = await this.vehicleRepository.GetVehiclesByPlate(plate);
            foreach (SpeedCAM vehicle in topVehicles)
            {

                string base64PlateImage = string.Empty;
             
                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                //if (!System.IO.File.Exists(vehicle.Plate_Image))
                //{
                //    base64PlateImage = clsHelps.imagenotfound;
                //}
                //if (!System.IO.File.Exists(vehicle.Full_Image))
                //{
                //    base64FullImage = clsHelps.imagenotfound;
                //}

                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = vehicle.Device,
                    Time = vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString() == "Away" ? "DT854" : "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }
        public async Task<List<VehicleDetailViewModel>> GetTopVehicleByVehiclesType(int type)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            IEnumerable<SpeedCAM> topVehicles = await this.vehicleRepository.GetAllVehicleByType(type);
  
            foreach (SpeedCAM vehicle in topVehicles)
            {

                string base64PlateImage = string.Empty;
                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                //if (!System.IO.File.Exists(vehicle.Plate_Image))
                //{
                //    base64PlateImage = clsHelps.imagenotfound;
                //}
                //if (!System.IO.File.Exists(vehicle.Full_Image))
                //{
                //    base64FullImage = clsHelps.imagenotfound;
                //}
                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = vehicle.Device,
                    Time = vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString() == "Away" ? "DT854" : "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }
        public async Task<List<VehicleDetailViewModel>> GetTopVehicleByDirection(int direction)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            IEnumerable<SpeedCAM> topVehicles = null;
            if (direction == 1)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesByDirection("Away");
            }
            else if (direction == 2)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesByDirection("Approach");
            }
            foreach (SpeedCAM vehicle in topVehicles)
            {

                string base64PlateImage = string.Empty;
                //if (File.Exists(vehicle.Full_Image)) base64PlateImage = Utility.fileToBase64(vehicle.Full_Image);
                //else logger.Write("GetTopVehicle", "File " + vehicle.Region + " not found", Logger.LogType.ERROR);

                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                //if (!System.IO.File.Exists(vehicle.Plate_Image))
                //{
                //    base64PlateImage = clsHelps.imagenotfound;
                //}
                //if (!System.IO.File.Exists(vehicle.Full_Image))
                //{
                //    base64FullImage = clsHelps.imagenotfound;
                //}
                //if (File.Exists(vehicle.Plate_Image)) base64FullImage = Utility.fileToBase64(vehicle.Plate_Image);

                //else logger.Write("GetTopVehicle", "File " + vehicle.Region + " not found", Logger.LogType.ERROR);

                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = vehicle.Device,
                    Time = vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString() == "Away" ? "DT854" : "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }
        public async Task<List<VehicleDetailViewModel>> GetTopVehicleBySpeed(int speed)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            IEnumerable<SpeedCAM> topVehicles =null ;
            if (speed == 0)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesBySpeed(null,null);
            }else if(speed == 1)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesBySpeed(0, 55);
            }
            else if(speed == 2)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesBySpeed(55, 60);
            }
            else if (speed == 3)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesBySpeed(60, 70);
            }
            else if (speed == 4)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesBySpeed(70, 80);
            }
            else
            {
                topVehicles = await this.vehicleRepository.GetVehiclesBySpeed(80, 10000);
            }

            foreach (SpeedCAM vehicle in topVehicles)
            {

                string base64PlateImage = string.Empty;
                //if (File.Exists(vehicle.Full_Image)) base64PlateImage = Utility.fileToBase64(vehicle.Full_Image);
                //else logger.Write("GetTopVehicle", "File " + vehicle.Region + " not found", Logger.LogType.ERROR);

                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                //if (!System.IO.File.Exists(vehicle.Plate_Image))
                //{
                //    base64PlateImage = clsHelps.imagenotfound;
                //}
                //if (!System.IO.File.Exists(vehicle.Full_Image))
                //{
                //    base64FullImage = clsHelps.imagenotfound;
                //}
                //if (File.Exists(vehicle.Plate_Image)) base64FullImage = Utility.fileToBase64(vehicle.Plate_Image);

                //else logger.Write("GetTopVehicle", "File " + vehicle.Region + " not found", Logger.LogType.ERROR);

                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = vehicle.Device,
                    Time = vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString() == "Away" ? "DT854" : "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }
        public async Task<List<VehicleDetailViewModel>> GetTopVehicleByDateTime(string starttime ,string endtime)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            IEnumerable<SpeedCAM> topVehicles = await this.vehicleRepository.GetVehiclesByTime(starttime, endtime);
            
            foreach (SpeedCAM vehicle in topVehicles)
            {

                string base64PlateImage = string.Empty;
                //if (File.Exists(vehicle.Full_Image)) base64PlateImage = Utility.fileToBase64(vehicle.Full_Image);
                //else logger.Write("GetTopVehicle", "File " + vehicle.Region + " not found", Logger.LogType.ERROR);

                string base64FullImage = string.Empty;
                base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);
                if (!System.IO.File.Exists(vehicle.Plate_Image))
                {
                    base64PlateImage = clsHelps.imagenotfound;
                }
                if (!System.IO.File.Exists(vehicle.Full_Image))
                {
                    base64FullImage = clsHelps.imagenotfound;
                }
                
                VehicleDetailViewModel model = new VehicleDetailViewModel
                {
                    Id = vehicle.Id,
                    Device = vehicle.Device,
                    Time = vehicle.Time,
                    Plate = vehicle.Plate,
                    Type = vehicle.Type,
                    Speed = vehicle.Speed,
                    Direction = vehicle.Direction.ToString() == "Away" ? "DT854" : "Chợ cái tàu hạ",
                    Plate_Color = vehicle.Plate_Color,
                    Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                    Vehicle_Color = vehicle.Vehicle_Color,
                    Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                    Plate_Image = base64PlateImage,
                    Full_Image = base64FullImage
                };
                result.Add(model);
            }

            return result;
        }
    }
}
