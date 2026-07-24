using System;

namespace HGC.Models
{
    public class PcGpu
    {
        public string GpuManufacturer { get; set; } = "";
        public string Model { get; set; } = "";
        public string CardManufacturer { get; set; } = "";

        public bool HasSerialNumber { get; set; }
        public string SerialNumber { get; set; } = "";

        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }

        public string VramAmount { get; set; } = "";
        public string VramType { get; set; } = "";
        public string VramBusWidth { get; set; } = "";
        public string VramClock { get; set; } = "";

        public string CoreClock { get; set; } = "";

        public bool HasOverclock { get; set; }
        public string OverclockCoreClock { get; set; } = "";
        public string OverclockMemoryClock { get; set; } = "";
        public string PowerLimit { get; set; } = "";

        public bool HasCoreDetails { get; set; }
        public string Rops { get; set; } = "";
        public string Tmus { get; set; } = "";
        public string VertexShaders { get; set; } = "";
        public string PixelShaders { get; set; } = "";
        public string UnifiedShaders { get; set; } = "";
        public string CudaCores { get; set; } = "";
        public string StreamProcessors { get; set; } = "";
        public string RtUnits { get; set; } = "";
        public string TensorUnits { get; set; } = "";
        public string OtherCoreDetails { get; set; } = "";

        public string InterfaceType { get; set; } = "";
        public string InterfaceVersion { get; set; } = "";
    }
}