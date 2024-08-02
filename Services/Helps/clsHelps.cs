
using System.Runtime.CompilerServices;

namespace VVA.ITS.WebApp.Services.Helps
{
    public class clsHelps
    {
      
        public static string imagenotfound = "/img/loading_plate_image.png";

        public static string GetRelativePath(string path)
        {
            int index = path.IndexOf("IMGS");

            if (index != -1)
            {
                string relativePath = path.Substring(index);
                return "\\"+ relativePath;
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetVehicleTypeString(int code)
        {
            switch (code)
            {
                case 1:
                    return "Xe khách từ 12 đến dưới 25 ghế";
                case 2:
                    return "Xe khách từ 25 đến 30 ghế";
                case 3:
                    return "Xe khách từ 31 ghế trở lên";
                case 4:
                    return "Xe buýt dưới 25 ghế";
                case 5:
                    return "Xe buýt từ 25 ghế trở lên";
                case 6:
                    return "Xe tải (2 trục, 4 bánh) tải trọng dưới 2 tấn";
                case 7:
                    return "Xe tải (2 trục, 4 bánh) tải trọng từ 2 tấn đến dưới 4 tấn";
                case 8:
                    return "Xe tải (2 trục, 6 bánh) tải trọng từ 2 tấn đến dưới 4 tấn";
                case 9:
                    return "Xe tải (2 trục, 6 bánh) tải trọng từ 4 tấn đến dưới 10 tấn";
                case 10:
                    return "Xe tải (2 trục, 6 bánh) tải trọng từ 10 tấn đến dưới 18 tấn";
                case 11:
                    return "Xe tải 3 trục; có tải trọng từ 4 tấn đến dưới 10 tấn";
                case 12:
                    return "Xe tải ≥ 3 trục; có tải trọng từ 10 tấn đến dưới 18 tấn";
                case 13:
                    return "Xe tải ≥ 3 trục; có tải trọng lớn hơn 18 tấn";
                case 14:
                    return "Xe đầu kéo kéo sơ-mi-rơ-moóc (≥ 3 trục)/ xe chở hàng bằng container 20 feet";
                case 15:
                    return "Xe đầu kéo kéo sơ-mi-rơ-moóc (≥ 3 trục)/ xe chở hàng bằng container 40 feet";
                default:
                    return "Unknown vehicle type";
            }
        }
        public static string GetRelativePath1(string path)
        {
            int index = path.IndexOf("upload");

            if (index != -1)
            {
                string relativePath = path.Substring(index);
                return "\\"+ relativePath;
            }
            else
            {
                return string.Empty;
            }
        }
        public static decimal KilogramsToTons(int? kilograms)
        {
            if (kilograms.HasValue)
            {
                decimal tons = (decimal)kilograms / 1000.0m; 
                return Math.Round(tons, 2);
            }
            else
            {
                return 0.00m;
            }
        }
        public static decimal khoiluongquatai(decimal KLsausaiso, decimal KLCP)
        {
            decimal kq = KLsausaiso - KLCP;
            if (kq <= 0)
            {
                return 0;
            }
            return kq;
        }

        public static decimal phantramquatrai(decimal khoiluongquatai, decimal TTchophep)
        {
            if (khoiluongquatai <= 0 || TTchophep <= 0)
            {
                return 0;
            }
            return Math.Round(khoiluongquatai / TTchophep, 2);
        }
        public static string getsocautruc(int check)
        {
            switch (check)
            {
                case 1:
                    return "1-1";
                case 2:
                    return "1-2";
                case 3:
                    return "1-1-1";
                case 4:
                    return "1-1-2";
                case 5:
                    return "1-3";
                case 6:
                    return "1-1-3";
                case 7:
                    return "1-1-1";
                case 8:
                    return "1-1-2";
                case 9:
                    return "1-2-1";
                case 10:
                    return "1-1-3";
                case 11:
                    return "1-2-2";
                case 12:
                    return "1-2-3";
                default: return "Không xác định";
            }
        }
        public static int gettrucxe(int check)
        {
            switch (check)
            {
                case 1:
                    return 2;
                case 2:
                case 3:
                    return 3;
                case 4:
                case 5:
                    return 4;
                case 6:
                    return 5;
                case 7:
                    return 3;
                case 8:
                case 9:
                    return 4;
                case 10:
                case 11:
                    return 5;
                case 12:
                    return 6;
                default:
                    return 0;
            }
        }
        public static string getpathimage(int check)
        {
            return "/img/"+check.ToString()+".png";
        }
        public static string GetSpeed(int speed)
        {
            if (speed > 55)
            {
                if (speed < 60)
                {
                    return speed.ToString()+ "km/h" + " Xe vi phạm tốc độ 5 - <10km/h";
                }
                else if (speed < 70)
                {
                    return speed.ToString() + "km/h" + " Xe vi phạm tốc độ 10 - <20km/h";
                }
                else if (speed < 80)
                {
                    return speed.ToString() + "km/h" + " Xe vi phạm tốc độ 20 - <35km/h";
                }
                else
                {
                    return speed.ToString() + "km/h" + " Xe vi phạm tốc độ >35km/h";
                }
            }
            else
            {
                return speed.ToString() + "km/h" + " Xe không vi phạm";
            }
        }
        public static string getDeviceType(int directionSend)
        {
            switch (directionSend)
            {
                case 1:
                    return "Speed CAM";
                case 2:
                    return "VL-NVVOI";
                case 3:
                    return "CTHa-DT854";
                case 4:
                    return "DT854-CTHa";
                case 5:
                    return "237PVC";
                default:
                    return "Speed CAM";
            }
        }
        public static string getSpeedCondition(int speedsend)
        {
            switch (speedsend)
            {
                case 1:
                    return " and speed < 55 ";
                case 2:
                    return " and speed >= 55 and speed < 60 ";
                case 3:
                    return " and speed >= 60 and speed < 70 ";
                case 4:
                    return " and speed >= 70 and speed < 80 ";
                case 5:
                    return " and speed >= 80 ";
                default:
                    return " and speed < 55 ";
            }
        }
        public static string getVehicleTypeCondition(int vehicleType)
        {
            switch (vehicleType)
            {
                case 1:
                    return " and vehicle_type = 'Motorbike'";
                case 2:
                    return " and vehicle_type = 'CAR'";
                case 3:
                    return " and vehicle_type = 'VAN'";
                case 4:
                    return " and vehicle_type = 'SUV'";
                case 5:
                    return " and vehicle_type = 'Truck'";
                case 6:
                    return " and vehicle_type = 'BUS'";
                case 7:
                    return " and (vehicle_type ='Other' or vehicle_type ='-')"; // Assuming undefined means vehicle type is not set or unknown
                default:
                    return " and vehicle_type = 'Motorbike'";
            }
        }
        public static string getPlateColor(int PlateColor)
        {
            switch (PlateColor)
            {
                case 1:
                    return " and plate_color = 'Red' ";
                case 2:
                    return " and plate_color = 'White' ";
                case 3:
                    return " and plate_color = 'Yellow' ";
                case 4:
                    return " and plate_color = 'Blue' ";
                default:
                    return " and plate_color = 'Red' ";
            }
        }
        public static string getVeheicleColor(int VehicleColor)
        {
            switch (VehicleColor)
            {
                case 1:
                    return " and vehicle_color = 'Red' ";
                case 2:
                    return " and vehicle_color = 'White' ";
                case 3:
                    return " and vehicle_color = 'Black' ";
                case 4:
                    return " and vehicle_color = 'Yellow' ";
                case 5:
                    return " and vehicle_color = 'Gray' ";
                case 6:
                    return " and vehicle_color = 'Blue' ";
                case 7:
                    return " and vehicle_color = 'Green' ";
                case 8:
                    return " and vehicle_color = '-' ";
                default:
                    return " and vehicle_color = 'Red' ";
            }
        }
        public static string getVehicleBrand(int VehicleBrand)
        {
            switch (VehicleBrand)
            {
                case 1:
                    return " and vehicle_brand = 'Audi' ";
                case 2:
                    return " and vehicle_brand = 'Land Rover' ";
                case 3:
                    return " and vehicle_brand = 'Isuzu' ";
                case 4:
                    return " and vehicle_brand = 'Bentley' ";
                case 5:
                    return " and vehicle_brand = 'Daewoo' ";
                case 6:
                    return " and vehicle_brand = 'Chevrolet' ";
                case 7:
                    return " and vehicle_brand = 'Honda' ";
                case 8:
                    return " and vehicle_brand = 'Ford' ";
                case 9:
                    return " and vehicle_brand = 'Nissan' ";
                case 10:
                    return " and vehicle_brand = 'Toyota' ";
                case 11:
                    return " and vehicle_brand = 'Mitsubishi' ";
                case 12:
                    return " and vehicle_brand = 'BMW' ";
                case 13:
                    return " and vehicle_brand = 'Volvo' ";
                case 14:
                    return " and vehicle_brand = 'Mazda' ";
                case 15:
                    return " and vehicle_brand = 'Hyundai' ";
                case 16:
                    return " and vehicle_brand = 'Kia' ";
                case 17:
                    return " and vehicle_brand = 'Aston Martin' ";
                case 18:
                    return " and vehicle_brand = 'Jeep' ";
                case 19:
                    return " and vehicle_brand = 'Peugeot' ";
                case 20:
                    return " and vehicle_brand = 'Dodge' ";
                case 21:
                    return " and vehicle_brand = 'BYD' ";
                case 22:
                    return " and vehicle_brand = '-' ";
                default:
                    // Giá trị mặc định khi không có hãng xe phù hợp
                    return " and vehicle_brand = 'Audi' ";
            }
        }

    }
}
