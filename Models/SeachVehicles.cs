namespace VVA.ITS.WebApp.Models
{
    public class SeachVehicles
    {
        public int VehicleType { get; set; } = 0;
        public string Plate { get; set;}=string.Empty;
        public int TypeViolation { get; set; } = 0;
        public string starttime { get; set; }=string.Empty;
        public string endtime { get; set; } = string.Empty;
        public int ViolationRatio { get; set; } = 0;
        public int ListStation { get; set; } = 0;

    }
}
