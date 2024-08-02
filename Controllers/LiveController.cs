using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Presentation;
using System.Globalization;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.Services;
using VVA.ITS.WebApp.Services.Helps;

namespace VVA.ITS.WebApp.Controllers
{
   
    public class LiveController : Controller
	{
        private readonly IDataxe? dataxe;
        private readonly UserManager<AppUser> userManager;
        private readonly IPdfService _pdfService;
        private readonly IWebHostEnvironment env;
        public LiveController(UserManager<AppUser> userManager, IDataxe dataxe, IPdfService pdfService, IWebHostEnvironment env)
		{
			this.dataxe = dataxe;
            this.userManager = userManager;
            this._pdfService = pdfService;
            this.env = env;
        }
        public async Task<IActionResult> Index()
		{
			try
			{
                AppUser user = await userManager.GetUserAsync(HttpContext.User);
                if (user == null) return RedirectToAction("Login", "Account");
                var Dataxe = await this.dataxe.GetAllDataXe();
                return View(Dataxe);
            }
            catch (Exception) { }
            return View();
        }
        [HttpPost]
		public async Task<List<DataXe>> SeachWeight([FromBody] SeachVehicles seachVehicles)
		{
			try
			{
                var data = await dataxe.SeachWeight(seachVehicles);
                return data;
            }
            catch (Exception) { }
            return null;
		}

        [HttpPost]
        public async Task<DataXe> SeachWeightById(string id)
        {
            try
            {
                var data = await dataxe.SeachWeightById(id);
                if (data != null)
                {
                    data.Hinhtruoc = clsHelps.GetRelativePath1(data.Hinhtruoc);
                    data.Hinhsau = clsHelps.GetRelativePath1(data.Hinhsau);
                }
                return data;
            }
            catch (Exception) { }
            return null;
        }
        [HttpPost]
        public async  Task<IActionResult>UpdateImage(IFormFileCollection files,string id)
        {
            try
            {
                var data = await dataxe.SeachWeightById(id);
                if (data==null) return BadRequest();

                if(data.Hinhsau.Length> 0 || data.Hinhtruoc.Length>0)
                {
                    return BadRequest();
                }
                string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "upload",DateTime.Now.ToString("ddMMyy"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                int i = 0;
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var path = Path.Combine(filePath, file.FileName);

                        if (!System.IO.File.Exists(path))
                        {
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            if (i == 0)
                            {
                                data.Hinhtruoc = path;
                            }
                            if (i == 1)
                            {
                                data.Hinhsau = path;
                            }
                            i++;
                        }
                    }
                }
                bool update = await dataxe.Update(data);
                if (update)
                {
                    return Ok("Câp nhật thành công.");
                }
            }
            catch (Exception) { }
            return BadRequest();
        }
       
        [HttpPost]
        public async Task<List<string>> UpdateImageFile(IFormFileCollection files)
        {
            List<string> data = new List<string>();
            try
            {
                string filePath = Path.Combine(this.env.ContentRootPath, "wwwroot", "upload", DateTime.Now.ToString("ddMMyy"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var path = Path.Combine(filePath, file.FileName);
                        if(!System.IO.File.Exists(path)) 
                        {
                            data.Add(path);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }
                    }
                }
               return data;
            }
            catch (Exception) { }
            return data;
        }
        [HttpPost]
        public async Task<IActionResult> ExportExel([FromBody] SeachVehicles seach)
        {
            try
            {
                AppUser user = await userManager.GetUserAsync(HttpContext.User);
                if (user != null)
                {
                    List<DataXe> data = await SeachWeight(seach);
                    if (user.UserName == "svtt1")
                    {
                        if (data != null && data.Count > 0)
                        {
                            string filePath = Path.Combine(env.ContentRootPath, "wwwroot", "exel", "phieucanxe.xlsx");

                            using (XLWorkbook wb = new XLWorkbook(filePath))
                            {
                                IXLWorksheet worksheet = wb.Worksheet(1);

                                if (worksheet != null)
                                {
                                    int row = 11; // Dòng bắt đầu ghi dữ liệu
                                    int i = 1;
                                    foreach (var vehicle in data)
                                    {
                                        //STT
                                        worksheet.Range($"A{row}:A{row + 1}").Merge();
                                        worksheet.Cell(row, 1).Value = i;
                                        i++;
                                        // Loại xe
                                        worksheet.Cell(row, 2).Value = clsHelps.GetVehicleTypeString((int)vehicle.Loaixe);
                                        worksheet.Range($"B{row}:B{row + 1}").Merge();

                                        // Kí hiệu
                                        worksheet.Cell(row, 3).Value = $"({vehicle.Loaixe})";
                                        worksheet.Range($"C{row}:C{row + 1}").Merge();

                                        // Biển kiểm soát
                                        worksheet.Cell(row, 4).Value = vehicle.Biensotruoc;
                                        worksheet.Range($"D{row}:D{row + 1}").Merge();

                                        // Số trục xe
                                        worksheet.Cell(row, 5).Value = 0;
                                        worksheet.Cell(row + 1, 5).Value = "K/cách trục";

                                        // Tải trọng trục
                                        worksheet.Cell(row, 6).Value = vehicle.Taitrongtruc1;
                                        worksheet.Cell(row, 7).Value = vehicle.Taitrongtruc2;
                                        worksheet.Cell(row, 8).Value = vehicle.Taitrongtruc3;
                                        worksheet.Cell(row, 9).Value = vehicle.Taitrongtruc4;
                                        worksheet.Cell(row, 10).Value = vehicle.Taitrongtruc5;
                                        worksheet.Cell(row, 11).Value = vehicle.Taitrongtruc6;

                                        worksheet.Cell(row + 1, 6).Value = 0;
                                        worksheet.Cell(row + 1, 7).Value = 0;
                                        worksheet.Cell(row + 1, 8).Value = 0;
                                        worksheet.Cell(row + 1, 9).Value = 0;
                                        worksheet.Cell(row + 1, 10).Value = 0;
                                        worksheet.Cell(row + 1, 11).Value = 0;

                                        // Tải trọng từng cụm trục
                                        worksheet.Range($"L{row}:L{row + 1}").Column(1).Merge();
                                        worksheet.Range($"M{row}:M{row + 1}").Column(1).Merge();
                                        worksheet.Range($"N{row}:N{row + 1}").Column(1).Merge();
                                        worksheet.Range($"O{row}:O{row + 1}").Column(1).Merge();
                                        worksheet.Range($"P{row}:P{row + 1}").Column(1).Merge();
                                        worksheet.Range($"Q{row}:Q{row + 1}").Column(1).Merge();
                                        worksheet.Range($"R{row}:R{row + 1}").Column(1).Merge();
                                        worksheet.Range($"S{row}:S{row + 1}").Column(1).Merge();

                                        worksheet.Cell(row, 12).Value = vehicle.TLtruc1;
                                        worksheet.Cell(row, 13).Value = vehicle.TLtruc2;
                                        worksheet.Cell(row, 14).Value = vehicle.TLtruc3;
                                        worksheet.Cell(row, 15).Value = vehicle.TTLtruc;
                                        worksheet.Cell(row, 16).Value = vehicle.Quataitong;
                                        worksheet.Cell(row, 17).Value = vehicle.Quataitheogp;

                                        // Tăng dòng để ghi dữ liệu cho xe tiếp theo
                                        row += 2;
                                    }

                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        wb.SaveAs(stream);
                                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"phieucan_{DateTime.Now.ToString("yyMMddHHmmss")}.xlsx");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (data != null && data.Count > 0)
                        {
                            string filePath = Path.Combine(env.ContentRootPath, "wwwroot", "exel", "baocaocanxe.xlsx");

                            using (XLWorkbook wb = new XLWorkbook(filePath))
                            {
                                IXLWorksheet worksheet = wb.Worksheet(1);

                                if (worksheet != null)
                                {
                                    int row = 6; 
                                    foreach (var vehicle in data)
                                    {
                                        worksheet.Cell(row, 1).Value = row-5;
                                        worksheet.Cell(row, 2).Value = vehicle.Thoigian;
                                        worksheet.Cell(row, 3).Value = clsHelps.gettrucxe(int.Parse(vehicle.Kieuxe));
                                        worksheet.Cell(row, 4).Value = clsHelps.getsocautruc(int.Parse(vehicle.Kieuxe));
                                        worksheet.Cell(row, 5).Value = vehicle.Biensotruoc;
                                        worksheet.Cell(row, 6).Value = vehicle.Biensosau;
                                        worksheet.Cell(row, 7).Value = vehicle.Tocdo;
                                        worksheet.Cell(row, 8).Value = vehicle.TTLtruc;
                                        worksheet.Cell(row, 9).Value = vehicle.TLgiayphep;
                                        worksheet.Cell(row, 10).Value = vehicle.Taitrongtruc1;
                                        worksheet.Cell(row, 11).Value = vehicle.Quataitruc1;
                                        worksheet.Cell(row, 12).Value = vehicle.Quataitruc2;
                                        worksheet.Cell(row, 13).Value = vehicle.Quataitruc3;
                                        worksheet.Cell(row, 14).Value = vehicle.TLtruc2;
                                        worksheet.Cell(row, 15).Value = vehicle.Quataitruc2;
                                        worksheet.Cell(row, 16).Value = vehicle.Taitrongtruc4;
                                        worksheet.Cell(row, 17).Value = vehicle.Taitrongtruc5;
                                        worksheet.Cell(row, 18).Value = vehicle.Taitrongtruc6;
                                        worksheet.Cell(row, 19).Value = vehicle.TLtruc3;
                                        worksheet.Cell(row, 20).Value = vehicle.Quataitruc3;
                                        worksheet.Cell(row, 21).Value = vehicle.Quataitong;
                                        row++;
                                    }
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        wb.SaveAs(stream);
                                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"baocaocanxe_{DateTime.Now.ToString("yyMMddHHmmss")}.xlsx");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Createpdf(string id)
        {
            try
            {
                
                var data = await dataxe.SeachWeightById(id);
                if (data!=null)
                {
                    if (data.Kieuxe.Length < 0)
                    {
                        data.Kieuxe = "0";
                    }
                    int check = int.Parse(data.Kieuxe);
                    string kieuxe = check > 6 ? "Container" : "Xe thân liền";
                    int truc = clsHelps.gettrucxe(check);
                    string Sogplx = data.Sogplx;
                    string stt = data.sttcan;
                    string chedocan = data.chedocan;
                    string html = "";
                    if ( check<7)
                    {
                        html += $@"<tr>
                                    <td>Ô tô</td>
                                    <td>{data.KT_oto_dai+"x"+data.KT_oto_rong+"x"+data.KT_oto_cao}</td>
                                    <td>{data.KTthunghang_dai + "x" + data.KTthunghang_rong + "x" + data.KTthunghang_cao}</td>
                                    <td>{data.Chieudaicoso}</td>
                                </tr>
                                <tr>
                                    <td>SMRM/RM</td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>";
                    }
                    else
                    {
                        html += $@"<tr>
                                    <td>Ô tô</td>
                                    <td>{data.KT_oto_dai + "x" + data.KT_oto_rong + "x" + data.KT_oto_cao}</td>
                                    <td>{data.KTthunghang_dai + "x" + data.KTthunghang_rong + "x" + data.KTthunghang_cao}</td>
                                    <td>{data.Chieudaicoso}</td>
                                </tr>
                                <tr>
                                    <td>SMRM/RM</td>
                                    <td>{data.KT_rm_dai + "x" + data.KT_rm_rong + "x" + data.KT_rm_cao}</td>
                                    <td>{data.KTthunghang_dai + "x" + data.KTthunghang_rong + "x" + data.KTthunghang_cao}</td>
                                    <td>{data.Chieudaicoso}</td>
                                </tr>";
                    }
                    string vipham = "";
                    if (data.Quataitruc1 <=0 && data.Quataitruc2 <= 0 && data.Quataitruc3 <= 0 && data.Quataitong <= 0 && data.Quataitheogp <= 0)
                    {
                        vipham += $@"<th>Không vi phạm</th>";
                    }
                    else
                    {
                        if (data.Quataitong >0)
                        {
                            vipham += $@"<th>Xe vượt tổng trọng lượng cho phép của cầu, đường: {data.Quataitong.ToString("0.00")} %</th>";
                            vipham += $@"<br>";
                        
                        }
                        double maxquatai = maxquatai = Math.Max(data.Quataitruc1, Math.Max(data.Quataitruc2, data.Quataitruc3));
                        if (maxquatai > 0)
                        {
                            vipham += $@"<th>Xe vượt tải trọng trục cho phép, đường: {maxquatai.ToString("0.00")} %</th>";
                            vipham += $@"<br>";
                        }
                       
                        if (data.Quataitheogp>0)
                        {
                            vipham += $@"<th>Xe vượt khối lượng hàng CC CPTGGT: {data.Quataitheogp.ToString("0.00")} %</th>";
                            vipham += $@"<br>";
                        }
                    }
                    decimal truc1 = clsHelps.KilogramsToTons(data.TLtruc1);
                    decimal truc2 = clsHelps.KilogramsToTons(data.TLtruc2);
                    decimal truc3 = clsHelps.KilogramsToTons(data.TLtruc3);
                    decimal truc1FivePercent = Math.Round(truc1 * 0.05m, 2);
                    decimal truc2FivePercent = Math.Round(truc2 * 0.05m, 2);
                    decimal truc3FivePercent = Math.Round(truc3 * 0.05m, 2);

                    decimal truc_sau1 = truc1 - truc1FivePercent;
                    decimal truc_sau2 = truc2 - truc2FivePercent;
                    decimal truc_sau3 = truc3 - truc3FivePercent;

                    decimal kltoanbo = truc1 + truc2 + truc3;
                    decimal klsaiso = truc1FivePercent + truc2FivePercent + truc3FivePercent;
                    decimal klsausaiso = truc_sau1 + truc_sau2 + truc_sau3;


                    decimal vuothanghoa = klsausaiso - clsHelps.KilogramsToTons(data.TLgiayphep)< 0?0: klsausaiso - clsHelps.KilogramsToTons(data.TLgiayphep);
                    
                    string checkdata = "";
                    if (check==1)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1,10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                                <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 1: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>16</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 2)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 1: d>= 1,3m</td>
                                                                <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>26</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 3)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                                <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>24</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 4)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>24</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 24)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 24), 24)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>30</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 5)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                              <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                               <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>30</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 6)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                              <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                               <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>34</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 7)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                              <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                               <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>26</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 8)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                              <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                              <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>34</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 9)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                             <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>34</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 10)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                             <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                               <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                 <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>42</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 11)
                    {
                        checkdata += $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                               <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 10), 10)}</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                             <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>24</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 24)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 24), 24)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>42</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    if (check == 12)
                    {
                        checkdata +=                        $@"<tr>
                                                                <td>Đơn 1:</td>
                                                                <td>{truc1}</td>
                                                                <td>{truc1FivePercent}</td>
                                                                <td>{truc_sau1}</td>
                                                                <td>10</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau1, 10)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau1, 10), 10)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Đơn 2:</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                           <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                                <td>{truc2}</td>
                                                                <td>{truc2FivePercent}</td>
                                                                <td>{truc_sau2}</td>
                                                                <td>18</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau2, 18)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau2, 18), 18)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Kép 2: d>= 1,3m</td>
                                                               <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Ba/ bốn: d> 1,3</td>
                                                                <td>{truc3}</td>
                                                                <td>{truc3FivePercent}</td>
                                                                <td>{truc_sau3}</td>
                                                                <td>24</td>
                                                                <td>{clsHelps.khoiluongquatai(truc_sau3, 24)}</td>
                                                                <td>{clsHelps.phantramquatrai(clsHelps.khoiluongquatai(truc_sau3, 24), 24)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng toàn bộ</td>
                                                                <td>{kltoanbo}</td>
                                                                <td>{klsaiso}</td>
                                                                <td>{klsausaiso}</td>
                                                                <td>42</td>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>";
                    }
                    //string path =  Path.Combine(this.env.ContentRootPath, "wwwroot", "img", "xx.png");
                    //<img src=""{path}"" alt=""Hình ảnh 1""  width=""400"" height=""300"">
                    //                                       <img src=""{path}"" alt=""Hình ảnh 2""  width=""400"" height=""300"">
                    string imgage = "";
                    string path = "", path1="";
                    if (System.IO.File.Exists(data.Hinhtruoc) && System.IO.File.Exists(data.Hinhsau))
                    {
                        path = data.Hinhtruoc;
                        path1 = data.Hinhsau;
                        imgage = $@"  <img src=""{path}"" alt=""Hình ảnh 1""  width=""400"" height=""300"">
                                                            <img src=""{path1}"" alt=""Hình ảnh 2""  width=""400"" height=""300"">";
                    }
                   
                    string htmlContent = $@"
                                        <!DOCTYPE html>
                                                    <html>
                                                    <head>
                                                        <title>Phiếu cân kiểm tra trọng tải</title>
                                                    </head>
                                                    <body>
                                                        <table>
                                                            <tr>
                                                                <td style=""color: red;"">SỞ GIAO THÔNG VẬN TẢI HỒ CHÍ MINH</td>
                                                                <td>Lý trình</td>
                                                                <td>Lần cân</td>
                                                              </tr>
                                                              <tr>
                                                                <td style=""color: red;"">Trạm CĐ 005 HCM</td>
                                                                <td>Số phiếu cân {data.Thoigian.ToString("ddMMyyy_hhmmss")}</td>
                                                                <td>{stt}</td>
                                                              </tr>
                                                        </table>
                                                        <h3 style=""text-align: center;"">PHIẾU CÂN KIỂM TRA TRỌNG TẢI</h3>
                                                        <div class=""container"">
                                                          {imgage}
                                                        </div>
                                                        <table>
                                                            <tr>
                                                                <td>BKS xe ô tô: {data.Biensotruoc}</td>
                                                                <td colspan=""3"">BKS SMRM/RM: {data.Biensosau}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Loại xe:{kieuxe}</td>
                                                                <td>Tổng số trục:{truc}</td>
                                                                <td>Mầu xe phía trước:{data.Mauxe}</td>
                                                                <td>Xe xi téc chở chất lỏng</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Tên chủ xe, địa chỉ:</td>
                                                                <td colspan=""3"">{data.Chuxe}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Họ tên lái xe:{data.Laixe}</td>
                                                                <td colspan=""2"">Số GPLX:{Sogplx}</td>
                                                                <td colspan=""2"">Số GPLHX:{data.Sogplhx}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Tốc độ xe qua cân [km/h]:{data.Tocdo}</td>
                                                                <td>Thời gian cân:{data.Thoigian.ToString("HH:mm:ss")}</td>
                                                                <td>Ngày cân:{data.Thoigian.ToString("dd/MM/yy")}</td>
                                                                <td>Chế độ cân:Động</td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan=""4"" style=""text-align: center;""><strong>THÔNG TIN KHỐI LƯỢNG VÀ KÍCH THƯỚC CHO PHÉP CỦA XE</strong></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng bản thân của ô tô [tấn]:{clsHelps.KilogramsToTons(data.Taitrongdaukeo)}</td>
                                                                <td></td>
                                                                <td>Khối lượng bản thân của SMRM/RM [tấn]:{clsHelps.KilogramsToTons(data.Taitrongromoc)}</td>
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng số người cho phép ngồi trên ô tô [tấn]: [0,065 tấn x số người] {data.Songuoitrenxe}</td>
                                                                <td colspan=""3"">{data.Songuoitrenxe * 65}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Khối lượng HHCC cho phép TGGT của ô tô/SMRM/RM [tấn]:</td>
                                                                <td colspan=""3"">{clsHelps.KilogramsToTons(data.Taitrongchophep)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td>Loại xe</td>
                                                                <td>KT bao (D x R x C) [m]</td>
                                                                <td>KT thùng hàng (D x R x C) [m]</td>
                                                                <td>Chiều dài CS [m]</td>
                                                            </tr>
                                                            {html}
                                                        </table>
                                                       
                                                        <table >
                                                            <tr>
                                                               <td colspan=""7"" style=""text-align: center;""><strong>I. KẾT QUẢ CÂN KIỂM TRA XE THEO TẢI TRỌNG (TT) CHO PHÉP CỦA CẦU, ĐƯỜNG</strong></td>
                                                            </tr>
                                                            <tr>
                                                                <th>Loại trục xe</th>
                                                                <th>TT cân được [tấn]</th>
                                                                <th>Sai số [tấn]</th>
                                                                <th>Sau trừ sai số [tấn]</th>
                                                                <th>TT cho phép [tấn]</th>
                                                                <th>Khối lượng quá tải [tấn]</th>
                                                                <th>Phần trăm quá tải (%)</th>
                                                            </tr>
                                                            {checkdata}
                                                        </table>
                                                        
                                                        <table>
                                                            <tr>
                                                                <td colspan=""4"" style=""text-align: center;""><strong>II. KẾT QUẢ CÂN KIỂM TRA XE THEO KHỐI LƯỢNG (KL) HÀNG CCCP TGGT</strong></td>
                                                            </tr>
                                                            <tr>
                                                                <th>KL hàng CC cân được [Tấn]</th>
                                                                <th>Vượt KL hàng CCCP TGGT [Tấn]</th>
                                                                <th>Phần trăm KL HH CC vượt [%]</th>
                                                            </tr>
                                                            <tr>
                                                                <td>{clsHelps.KilogramsToTons(data.TLgiayphep)}</td>
                                                                <td>{vuothanghoa}</td>
                                                                <td>{data.Quataitheogp}</td>
                                                            </tr>
                                                        </table>
                                                        <table>
                                                            <tr>
                                                                 <td colspan=""4"" style=""text-align: center;""><strong>III. KẾT QUẢ ĐO LƯỜNG KT BAO, THÙNG HÀNG (D x R x C) [m]</strong></td>
                                                            </tr>
                                                            <tr>
                                                                <th>Kích thước bao vượt</th>
                                                                <th>Kích thước thùng vượt</th>
                                                            </tr>
                                                                <td>0</td>
                                                                <td>0</td>
                                                            </tr>
                                                        </table>
                                                        <h4>IV. KẾT LUẬN</h4>
                                                        {vipham}
                                                    </body>
                                                    </html>
                                        ";

                    var pdfBytes = _pdfService.GeneratePdfWeigt(htmlContent);
                    return File(pdfBytes, "application/pdf");
                }
            }
            catch (Exception) { }
            return BadRequest();
        }
    }
}
