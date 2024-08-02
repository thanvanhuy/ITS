using Microsoft.AspNetCore.SignalR;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.ViewModels;
using AppUtilObjectCore;
using VVA.ITS.WebApp.Services.Helps;


namespace VVA.ITS.WebApp.Hubs
{
    public  class DashboardHub : Hub
    {
        private IHubContext<DashboardHub> hubContext;
        private IVehicleRepository vehicleRepository;
        private readonly IWebHostEnvironment env;
        private Logger logger;
        private static readonly Dictionary<string, string> UserConnections = new Dictionary<string, string>();
        public static bool checkfind { get; set; } = false;
      
        public  DashboardHub(IHubContext<DashboardHub> hubContext,
                                            IVehicleRepository vehicleRepository, 
                                            IWebHostEnvironment env)
        {
          
            this.vehicleRepository = vehicleRepository;
            this.hubContext = hubContext;
            this.env = env;
            this.logger = new Logger();
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendVehicles()
        {
            
            List<VehicleDetailViewModel> vehicles = await this.GetTopVehicle(10);
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
            foreach (Speed_CAM_NEW vehicle in topVehicles)
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
        public async Task<List<VehicleDetailViewModel>> GetTopVehicle(int number)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            // Biến để lưu trữ các xe hàng đầu từ các người dùng khác nhau
            IEnumerable<SpeedCAM> topVehicles = new List<SpeedCAM>();
            topVehicles = await this.vehicleRepository.GetTopVehicles(number, false);
                if (topVehicles != null)
                {
                    foreach (SpeedCAM vehicle in topVehicles)
                    {
                        string base64PlateImage = clsHelps.GetRelativePath(vehicle.Plate_Image);
                        string base64FullImage = clsHelps.GetRelativePath(vehicle.Full_Image);

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
                            Direction = vehicle.Direction.ToString(),
                            Plate_Color = vehicle.Plate_Color,
                            Vehicle_Type = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "Xe máy" : "Ô tô",
                            Vehicle_Color = vehicle.Vehicle_Color,
                            Vehicle_Brand = vehicle.Vehicle_Type.ToString() == "Motorbike" ? "" : vehicle.Vehicle_Brand,
                            Plate_Image = base64PlateImage,
                            Full_Image = base64FullImage
                        };
                        result.Add(model);
                    }
                }
            

            return result;
        }

        public async Task<List<VehicleDetailViewModel>> GetTopVehicleByPlate(string plate)
        {
            List<VehicleDetailViewModel> result = new List<VehicleDetailViewModel>();
            var topVehicles = await this.vehicleRepository.GetVehiclesByPlate(plate);
            foreach (Speed_CAM_NEW vehicle in topVehicles)
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
            IEnumerable<Speed_CAM_NEW> topVehicles = await this.vehicleRepository.GetAllVehicleByType(type);
  
            foreach (Speed_CAM_NEW vehicle in topVehicles)
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
            IEnumerable<Speed_CAM_NEW> topVehicles = null;
            if (direction == 1)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesByDirection("Away");
            }
            else if (direction == 2)
            {
                topVehicles = await this.vehicleRepository.GetVehiclesByDirection("Approach");
            }
            foreach (Speed_CAM_NEW vehicle in topVehicles)
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
            IEnumerable<Speed_CAM_NEW> topVehicles =null ;
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

            foreach (Speed_CAM_NEW vehicle in topVehicles)
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
            IEnumerable<Speed_CAM_NEW> topVehicles = await this.vehicleRepository.GetVehiclesByTime(starttime, endtime);
            
            foreach (Speed_CAM_NEW vehicle in topVehicles)
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
