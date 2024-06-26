using Microsoft.AspNetCore.Mvc;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using AppUtilObjectCore;
using VVA.ITS.WebApp.Services.Helps;
using ClosedXML.Excel;

// Reference (AJAX ASP.NET core 6.0): https://www.c-sharpcorner.com/article/ajax-in-net-core/
// Reference (ASP.NET with SignalR): https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-6.0&tabs=visual-studio
// Reference (Database change notification with SignalR): https://www.freecodespot.com/blog/display-database-change-notification-using-signalr/

namespace VVA.ITS.WebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IVehicleRepository vehicleRepository;
        
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        private Logger logger;
        private readonly IPdfService _pdfService;
        public DashboardController(IVehicleRepository vehicleRepository,
                                                   
                                                    UserManager<AppUser> userManager,
                                                    IConfiguration configuration,
                                                    IPdfService pdfService,
                                                    IWebHostEnvironment env)
        {
            this.vehicleRepository = vehicleRepository;
           
            this.userManager = userManager;
            this.configuration = configuration;
            this.env = env;
            this._pdfService = pdfService;
        }

        //[Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                AppUser user = await userManager.GetUserAsync(HttpContext.User);
                if (user == null) return RedirectToAction("Login", "Account");
                var vehicles = await this.vehicleRepository.GetTopVehicles(50);
                List<VehicleDetailViewModel> vehicleVMs = new List<VehicleDetailViewModel>();
                foreach (SpeedCAM vehicle in vehicles)
                {
                    VehicleDetailViewModel vehicleVM = new VehicleDetailViewModel();
                    // Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "unknown.png")
                    vehicleVM.GetData(vehicle);
                    vehicleVMs.Add(vehicleVM);
                }

                //var locations = await this.locationRepository.GetAllLocations();
                DashboardViewModel dashboardVM = new DashboardViewModel
                {
                    hostAddress = this.configuration.GetValue<string>("TcpServer:IPAddress"),
                    hostPort = this.configuration.GetValue<string>("TcpServer:Port"),
                    vehicles = vehicleVMs,
                    //locations = locations
                };
                return View(dashboardVM);
            }
            catch (Exception ex)
            {
                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.Index", ex.Message, Logger.LogType.ERROR);
            }
            return View();
        }
        //[HttpPost]
        //public async Task<JsonResult> GetVehicleDetailsById(IFormCollection formCollection)
        //{
        //    try
        //    {
        //        int vehicleID = Utility.getInt(formCollection["vehicleID"]);
        //        var vehicle = await this.vehicleRepository.GetVehicle(vehicleID);
        //        JsonResponseViewModel viewModel = new JsonResponseViewModel();
        //        if (vehicle != null)
        //        {
        //            if (System.IO.File.Exists(vehicle.plate_image)) vehicle.plate_image = Utility.fileToBase64(vehicle.plate_image);
        //            else
        //            {                        
        //                if (this.logger == null) this.logger = new Logger();
        //                logger.Write("DashboardController.GetVehicleDetailsById()", "File " + vehicle.plate_image + " not found!", Logger.LogType.ERROR);
        //                string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "loading_plate_image.png");
        //                vehicle.plate_image = Utility.fileToBase64(filePath);
        //            }

        //            if (System.IO.File.Exists(vehicle.full_image)) vehicle.full_image = Utility.fileToBase64(vehicle.full_image);
        //            else
        //            {                        
        //                if (this.logger == null) this.logger = new Logger();
        //                logger.Write("DashboardController.GetVehicleDetailsById()", "File " + vehicle.full_image + " not found!", Logger.LogType.ERROR);
        //                string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "loading_full_image.png");
        //                vehicle.full_image = Utility.fileToBase64(filePath);
        //            }
        //            viewModel.responseCode = 0;
        //            viewModel.responseMessage = JsonConvert.SerializeObject(vehicle);
        //        }
        //        else
        //        {
        //            viewModel.responseCode = 1;
        //            viewModel.responseMessage = "No record available!";
        //        }
        //        return Json(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (this.logger == null) this.logger = new Logger();
        //        logger.Write("DashboardController.GetVehicleDetailsById()", ex.Message, Logger.LogType.ERROR);
        //    }
        //    return null;
        //}


        [HttpGet]
        public async Task<JsonResult> GetVehicleDetailsById(int id)
        {
            try
            {
                var vehicle = await this.vehicleRepository.GetVehicle(id);
                JsonResponseViewModel viewModel = new JsonResponseViewModel();
                if (vehicle != null)
                {
                    vehicle.Plate_Image = clsHelps.GetRelativePath(vehicle.Plate_Image);
                    vehicle.Full_Image = clsHelps.GetRelativePath(vehicle.Full_Image);
                    if (!System.IO.File.Exists(vehicle.Plate_Image))
                    {
                        vehicle.Plate_Image = clsHelps.imagenotfound;
                    }
                    if (!System.IO.File.Exists(vehicle.Full_Image))
                    {
                        vehicle.Full_Image = clsHelps.imagenotfound;
                    }
                    //if (System.IO.File.Exists(vehicle.Plate_Image)) vehicle.Plate_Image = Utility.fileToBase64(vehicle.Plate_Image);
                    //else
                    //{
                    //    if (this.logger == null) this.logger = new Logger();
                    //    logger.Write("DashboardController.GetVehicleDetailsById()", "File " + vehicle.Plate_Image + " not found!", Logger.LogType.ERROR);
                    //    string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "loading_plate_image.png");
                    //    vehicle.Plate_Image = Utility.fileToBase64(filePath);
                    //}

                    //if (System.IO.File.Exists(vehicle.Full_Image)) vehicle.Full_Image = Utility.fileToBase64(vehicle.Full_Image);
                    //else
                    //{
                    //    if (this.logger == null) this.logger = new Logger();
                    //    logger.Write("DashboardController.GetVehicleDetailsById()", "File " + vehicle.Full_Image + " not found!", Logger.LogType.ERROR);
                    //    string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "loading_plate_image.png");//loading_full_image
                    //    vehicle.Full_Image = Utility.fileToBase64(filePath);
                    //}
                    viewModel.responseCode = 0;
                    viewModel.responseMessage = JsonConvert.SerializeObject(vehicle);
                }
                else
                {
                    viewModel.responseCode = 1;
                    viewModel.responseMessage = "No record available!";
                }
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.GetVehicleDetailsById()", ex.Message, Logger.LogType.ERROR);
            }
            return null;
        }
        [HttpGet]
        public async Task<JsonResult> GetVehiclesByday()
        {
            try
            {
                var vehicles = await this.vehicleRepository.GetAllVehicles();
                JsonResponseViewModel viewModel = new JsonResponseViewModel();
                if (vehicles != null && vehicles.Any())
                {
                    Dictionary<string, int> vehiclesCountByDay = new Dictionary<string, int>();

                    foreach (var vehicle in vehicles)
                    {
                        string day = vehicle.Time.Date.ToString("dd/MM");

                        if (!vehiclesCountByDay.ContainsKey(day))
                        {
                            vehiclesCountByDay.Add(day, 1);
                        }
                        else
                        {
                            vehiclesCountByDay[day]++;
                        }
                    }

                    viewModel.responseCode = 0;
                    viewModel.responseMessage = JsonConvert.SerializeObject(vehiclesCountByDay);
                }
                else
                {
                    viewModel.responseCode = 1;
                    viewModel.responseMessage = "No record available!";
                }

                return Json(viewModel);
            }
            catch (Exception ex)
            {

                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.GetVehiclesByday()", ex.Message, Logger.LogType.ERROR);
                return null;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetVehiclesxemay()
        {
            try
            {
                var vehicles = await this.vehicleRepository.GetAllVehiclesxemayvipham();
                JsonResponseViewModel viewModel = new JsonResponseViewModel();
                if (vehicles != null && vehicles.Any())
                {
                    Dictionary<string, int> vehiclesCountByDay = new Dictionary<string, int>();

                    foreach (var vehicle in vehicles)
                    {
                        string day = vehicle.Time.Date.ToString("dd/MM");

                        if (!vehiclesCountByDay.ContainsKey(day))
                        {
                            vehiclesCountByDay.Add(day, 1);
                        }
                        else
                        {
                            vehiclesCountByDay[day]++;
                        }
                    }

                    viewModel.responseCode = 0;
                    viewModel.responseMessage = JsonConvert.SerializeObject(vehiclesCountByDay);
                }
                else
                {
                    viewModel.responseCode = 1;
                    viewModel.responseMessage = "No record available!";
                }

                return Json(viewModel);
            }
            catch (Exception ex)
            {

                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.GetVehiclesByday()", ex.Message, Logger.LogType.ERROR);
                return null;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetVehiclesoto()
        {
            try
            {
                var vehicles = await this.vehicleRepository.GetAllVehiclesoto();
                JsonResponseViewModel viewModel = new JsonResponseViewModel();
                if (vehicles != null && vehicles.Any())
                {
                    Dictionary<string, int> vehiclesCountByDay = new Dictionary<string, int>();

                    foreach (var vehicle in vehicles)
                    {
                        string day = vehicle.Time.Date.ToString("dd/MM");

                        if (!vehiclesCountByDay.ContainsKey(day))
                        {
                            vehiclesCountByDay.Add(day, 1);
                        }
                        else
                        {
                            vehiclesCountByDay[day]++;
                        }
                    }

                    viewModel.responseCode = 0;
                    viewModel.responseMessage = JsonConvert.SerializeObject(vehiclesCountByDay);
                }
                else
                {
                    viewModel.responseCode = 1;
                    viewModel.responseMessage = "No record available!";
                }

                return Json(viewModel);
            }
            catch (Exception ex)
            {

                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.GetVehiclesByday()", ex.Message, Logger.LogType.ERROR);
                return null;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetVehiclesotovipham()
        {
            try
            {
                var vehicles = await this.vehicleRepository.GetAllVehiclesotovipham();
                JsonResponseViewModel viewModel = new JsonResponseViewModel();
                if (vehicles != null && vehicles.Any())
                {
                    Dictionary<string, int> vehiclesCountByDay = new Dictionary<string, int>();

                    foreach (var vehicle in vehicles)
                    {
                        string day = vehicle.Time.Date.ToString("dd/MM");

                        if (!vehiclesCountByDay.ContainsKey(day))
                        {
                            vehiclesCountByDay.Add(day, 1);
                        }
                        else
                        {
                            vehiclesCountByDay[day]++;
                        }
                    }

                    viewModel.responseCode = 0;
                    viewModel.responseMessage = JsonConvert.SerializeObject(vehiclesCountByDay);
                }
                else
                {
                    viewModel.responseCode = 1;
                    viewModel.responseMessage = "No record available!";
                }

                return Json(viewModel);
            }
            catch (Exception ex)
            {

                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.GetVehiclesByday()", ex.Message, Logger.LogType.ERROR);
                return null;
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetVehicleUpdate()
        {
            try
            {
                var vehicles = await this.vehicleRepository.GetTopVehicles(50, false);
                string htmlTableData = string.Empty;
                foreach (SpeedCAM vehicle in vehicles)
                {
                    //vehicle.location = await this.locationRepository.GetLocation(vehicle.locationID);
                    htmlTableData += "<tr style='cursor:grab'>"
                                            + "<input type='hidden' value='" + vehicle.Id + "'/>"
                                            + "<td>" + "DT841" + "</td>"
                                            + "<td>" + vehicle.Time.ToString("dd/MM/yyyy HH:mm:ss") + "</td>"
                                            + "<td>" + vehicle.Plate + "</td>"
                                            + "<td>" + vehicle.Type + "</td>"
                                            + "<td>" + vehicle.Speed + "</td>"
                                            + "<td>" + vehicle.Vehicle_Type + "</td>"
                                            + "<td>" + vehicle.Vehicle_Color + "</td>"
                                            + "<td>" + vehicle.Vehicle_Brand + "</td>"
                                            + "</tr>";
                }
                GetVehicleUpdateViewModel getVehicleUpdateVM = new GetVehicleUpdateViewModel();
                getVehicleUpdateVM.listVehicle = htmlTableData;
                vehicles.First().Plate_Image = Utility.fileToBase64(vehicles.First().Plate_Image);
                vehicles.First().Full_Image = Utility.fileToBase64(vehicles.First().Full_Image);
                getVehicleUpdateVM.firstVehicle = vehicles.FirstOrDefault();
                JsonResponseViewModel viewModel = new JsonResponseViewModel()
                {
                    responseCode = 0,
                    responseMessage = JsonConvert.SerializeObject(getVehicleUpdateVM)
                };
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.GetVehicleUpdate()", ex.Message, Logger.LogType.ERROR);
            }
            return null;
        }
        [HttpPost]
        public async Task<IActionResult> Generatexml()
        {
            try
            {
                var vehicles = await this.vehicleRepository.GetVehiclesBySpeed(55, null);
                int row = 5;
                string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "exel", "xevipham.xlsx");

                using (XLWorkbook wb = new XLWorkbook(filePath))
                {
                    if (wb != null)
                    {
                        IXLWorksheet worksheet = wb.Worksheet(1);

                        if (worksheet != null)
                        {
                            foreach (var vehicle in vehicles)
                            {
                                row++;
                                worksheet.Cell($"A{row}").Value = (row).ToString();
                                worksheet.Cell($"B{row}").Value = vehicle.Time.ToString();
                                worksheet.Cell($"C{row}").Value = vehicle.Plate;
                                worksheet.Cell($"D{row}").Value = vehicle.Vehicle_Type?.ToString() == "Motorbike" ? "Xe máy" : "Ô tô";
                                worksheet.Cell($"E{row}").Value = clsHelps.GetSpeed((int)vehicle.Speed);
                                worksheet.Cell($"F{row}").Value = "Đang cập nhật";
                                worksheet.Cell($"G{row}").Value = "Đang cập nhật";
                            }

                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Baocaoxevipham_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Worksheet not found in the workbook.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Workbook not found at the specified location.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            try
            {
                var imagePath = Path.Combine(env.WebRootPath, "img", "csgt.png");
                //var imgtest= Path.Combine(env.WebRootPath, "img", "202404160613022491205.jpg");
              
                //Debug.WriteLine("xxxxxx" + id);
                // Tạo nội dung HTML từ các nội dung đã cung cấp
                var xe = await this.vehicleRepository.GetVehicle(id);
                var addspeed = "";
                var speed = xe.Speed;
                if (speed < 55)
                {
                    addspeed = " <p>Kết luận:Xe không vi phạm.</p>";
                }
                else
                {
                    if (speed < 60)
                    {
                        addspeed = " <p>Kết luận:Xe vi phạm tốc độ từ 5km/h đến 10km/h.</p>";
                    }
                    if (speed >= 60 && speed < 70)
                    {
                        addspeed = " <p>Kết luận:Xe vi phạm tốc độ từ 10km/h đến 20km/h.</p>";
                    }
                    if (speed >= 70 && speed < 80)
                    {
                        addspeed = " <p>Kết luận:Xe vi phạm tốc độ từ 20km/h đến 30km/h.</p>";
                    }
                    if (speed >= 80)
                    {
                        addspeed = " <p>Kết luận:Xe vi phạm tốc độ trên 35km/h.</p>";
                    }
                }
                var brand = xe.Vehicle_Type.ToString() == "Motorbike" ? "" : xe.Vehicle_Brand;
                var type = xe.Vehicle_Type == "Motorbike" ? "xe máy":"ô tô";
                var direction = xe.Direction == "Away" ? "DT854 đến xã Hòa Tân" : "Xã Hòa Tân đến Chợ cái tàu hạ";
                //Console.WriteLine(xe);
                string htmlContent = $@"
                                        <!DOCTYPE html>
                                        <html>
                                        <head>
                                            <title>Hình ảnh phương tiện vi phạm</title>
                                        </head>
                                        <body>
                                            <div class=""header"">
                                                <div class=""left"">
                                                    <p>CA TỈNH ĐỒNG THÁP</p>
                                                    <p>CA HUYỆN CHÂU THÀNH</p>
                                                    <img src=""{imagePath}"" alt=""Ảnh CSGT"" width=""100"" height=""100"">
                                                </div>
                                                <div class=""right"">
                                                    <p>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</p>
                                                    <p>Độc lập - Tự do - Hạnh phúc</p>
                                                </div>
                                            </div>
                                            <div class=""header1"">
                                                <p>HỆ THỐNG GIÁM SÁT, XỬ LÝ VI PHẠM TRẬT TỰ, AN TOÀN GIAO THÔNG ĐƯỜNG BỘ</p>
                                                <p>HÌNH ẢNH PHƯƠNG TIỆN VI PHẠM</p>
                                            </div>
                                            <div class=""data"">
                                               <p>Thời điểm vi phạm {xe.Time.ToString("dd/MM/yyyy HH:mm:ss")}.</p>
                                                <p>Địa điểm vi phạm:Đường DT854 khóm Phú Mỹ Thành TT Cái Tàu Hạ tỉnh Đồng Tháp.</p>
                                                <p>Tọa độ:10.252144868658531, 105.86876690131744.</p>
                                                <p>Loại phương tiện: {type}.</p>
                                                <p>Hãng xe {brand}.</p>
                                                <p>Hướng đi: {direction}.</p>
                                                <p>Tốc độ đo được: {xe.Speed} km/h.</p>
                                                <p>Tốc độ tối đa cho phép: 50 km/h.</p>
                                                {addspeed}                
                                                <p>Thiết bị giám sát:Camera GSGT-Radar.</p>
                                                <p>Hành vi vi phạm:Vi phạm tốc độ.</p>
                                                <p>Biển số: {xe.Plate}.</p>
                                                <p>Đơn vị vận hành hệ thống:Đội CSGT,TT CA huyện Châu Thành tỉnh Đồng Tháp</p>
                                                <p>Tên file ảnh: {xe.Plate_Image}</p>
                                                <img src=""{xe.Plate_Image}"" alt=""logo""  height=""400"">
                                            </div>
                                        </body>
                                        </html>
                                        ";
  
                var pdfBytes = _pdfService.GeneratePdf(htmlContent);

                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                if (this.logger == null) this.logger = new Logger();
                logger.Write("DashboardController.violation_details()", ex.Message, Logger.LogType.ERROR);
                return BadRequest();
            }
        }

    }
}