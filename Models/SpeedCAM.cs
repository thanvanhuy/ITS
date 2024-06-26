namespace VVA.ITS.WebApp.Models
{
    public class SpeedCAM
    {
        public int Id { get; set; }
        public string? Device { get; set; }
        public DateTime Time { get; set; }
        public string? Plate { get; set; }
        public string? Type { get; set; }
        public int? Speed { get; set; }
        public string? Direction { get; set; }
        public string? Region { get; set; }
        public string? Confidence { get; set; }
        public string? Plate_Color { get; set; }
        public string? Vehicle_Type { get; set; }
        public string? Vehicle_Color { get; set; }
        public string? Vehicle_Brand { get; set; }
        public string? Plate_Image { get; set; }
        public string? Full_Image { get; set; }
    }

}
