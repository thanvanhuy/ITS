
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

    }
}
