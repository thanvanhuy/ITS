﻿namespace VVA.ITS.WebApp.Models
{
    public  class Seach
    {
        public int speedsend { get; set; } = 0;
        public  int directionsend { get; set; } = 0;
        public string platesend { get; set; } = string.Empty;
        public  string starttime { get; set; }
        public  string endtime { get; set; }
        public  int VehicleType { get; set; } = 0;
    }
}