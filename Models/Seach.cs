namespace VVA.ITS.WebApp.Models
{
    public  class Seach
    {
        public int speedsend { get; set; } = 0;
        public  int directionsend { get; set; } = 0;
        public  int platecolor { get; set; } = 0;
        public  int vehiclecolor { get; set; } = 0;
        public  int vehiclebrand { get; set; } = 0;
        public string platesend { get; set; } = string.Empty;
        public  string starttime { get; set; } = DateTime.Now.AddMinutes(-5).ToString();
        public  string endtime { get; set; } = DateTime.Now.ToString();
        public  int VehicleType { get; set; } = 0;
    }
}
