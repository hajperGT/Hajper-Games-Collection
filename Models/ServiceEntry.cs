using System;

namespace HGC.Models
{
    internal class ServiceEntry
    {
        public DateTime? ServiceDate { get; set; }

        public string ServiceType { get; set; } = "";

        public string CustomNote { get; set; } = "";

        public decimal? Cost { get; set; }

        public string CostCurrency { get; set; } = "PLN";

    }
}