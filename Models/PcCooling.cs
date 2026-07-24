using System;
using System.Collections.Generic;

namespace HGC.Models
{
    internal class PcCooling
    {
        public string CoolingMode { get; set; } = "";
        // StandardCooling / CustomWaterLoop

        public PcStandardCooling StandardCooling { get; set; } = new();

        public PcCustomWaterLoop CustomWaterLoop { get; set; } = new();

        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }

        public string Notes { get; set; } = "";
    }

    internal class PcStandardCooling
    {
        public PcCpuCooling CpuCooling { get; set; } = new();
        public PcGpuCooling GpuCooling { get; set; } = new();
        public PcCpuAioCooling AioCooling { get; set; } = new();
        public PcMotherboardCooling MotherboardCooling { get; set; } = new();
        public PcSimpleCooling RamCooling { get; set; } = new();
        public PcSimpleCooling StorageCooling { get; set; } = new();
        public PcPsuCooling PsuCooling { get; set; } = new();
        public PcCaseCooling CaseCooling { get; set; } = new();
    }
    internal class PcCpuAioCooling
    {
        public bool HasAioDetails { get; set; }

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string Notes { get; set; } = "";
    }   
    internal class PcCpuCooling
    {
        public string CoolingType { get; set; } = "";

        public string OtherCoolingType { get; set; } = "";

        public PcCpuAirCooling AirCooling { get; set; } = new();

        public PcCpuAioCooling AioCooling { get; set; } = new();
    }

    internal class PcCpuAirCooling
    {
        public string RadiatorType { get; set; } = "";
        // NotSpecified / BoxOem / Tower / DualTower / TripleTower / TopFlow / LowProfile / SlotCooler / Passive / Other

        public string OtherRadiatorType { get; set; } = "";

        public string RadiatorPrimaryColor { get; set; } = "";
        public string RadiatorSecondaryColor { get; set; } = "";

        public bool HasPeltierModule { get; set; }
        public string PeltierModel { get; set; } = "";

        public bool HasHeatpipeCount { get; set; }
        public int? HeatpipeCount { get; set; }

        public PcCoolingFanSet Fans { get; set; } = new();
    }

    internal class PcGpuCooling
    {
        public string CoolingType { get; set; } = "";
        // NotSpecified / Stock / Custom

        public string StockType { get; set; } = "";
        // Air / Passive / HybridAio

        public string CustomDescription { get; set; } = "";
    }

    internal class PcMotherboardCooling
    {
        public string CoolingType { get; set; } = "";
        // NotSpecified / Stock / Custom / UsesCustomWaterLoop / StandardCooling

        public string StockType { get; set; } = "";
        // Air / Passive

        public string CustomDescription { get; set; } = "";

        public string WaterBlockManufacturer { get; set; } = "";
        public string WaterBlockModel { get; set; } = "";
        public string FittingsDescription { get; set; } = "";
    }

    internal class PcSimpleCooling
    {
        public string CoolingType { get; set; } = "";
        // NotSpecified / Stock / Custom / UsesCustomWaterLoop / StandardCooling

        public string StockType { get; set; } = "";
        // None / Passive / Active

        public string CustomDescription { get; set; } = "";

        public string WaterBlockDescription { get; set; } = "";
        public string FittingsDescription { get; set; } = "";
    }

    internal class PcPsuCooling
    {
        public string CoolingType { get; set; } = "";
        // NotSpecified / Stock / Custom / UsesCustomWaterLoop / StandardCooling

        public string StockType { get; set; } = "";
        // None / Passive / Active

        public PcCoolingFanSet Fans { get; set; } = new();

        public string CustomDescription { get; set; } = "";

        public string WaterBlockDescription { get; set; } = "";
        public string FittingsDescription { get; set; } = "";
    }

    internal class PcCaseCooling
    {
        public List<PcCaseFanGroup> FanGroups { get; set; } = new();
    }

    internal class PcCaseFanGroup
    {
        public string Location { get; set; } = "";
        // Front / LeftSide / RightSide / Top / Bottom / Rear

        public List<PcCoolingFan> Fans { get; set; } = new();
    }

    internal class PcCoolingFanSet
    {
        public bool HasFanDetails { get; set; }
        public int? FanCount { get; set; }

        public List<PcCoolingFan> Fans { get; set; } = new();
    }

    internal class PcCoolingFan
    {
        public string Size { get; set; } = "";
        // 80 / 92 / 100 / 120 / 135 / 140 / 180 / 200 / Other

        public string OtherSize { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";

        public string RgbType { get; set; } = "";
        // NoRgb / RGB / ARGB / FixedLed / Other

        public string OtherRgbType { get; set; } = "";

        public bool IsReverseFan { get; set; }
    }

    internal class PcCustomWaterLoop
    {
        public PcWaterLoopComponent CpuBlock { get; set; } = new();
        public PcWaterLoopComponent GpuBlock { get; set; } = new();
        public PcWaterLoopComponent MotherboardBlock { get; set; } = new();
        public PcWaterLoopComponent RamBlock { get; set; } = new();
        public PcWaterLoopComponent StorageBlock { get; set; } = new();
        public PcWaterLoopComponent PsuBlock { get; set; } = new();

        public PcWaterReservoir Reservoir { get; set; } = new();
        public PcWaterPump Pump { get; set; } = new();

        public List<PcWaterRadiator> Radiators { get; set; } = new();

        public PcWaterTubing Tubing { get; set; } = new();
        public PcWaterFittings Fittings { get; set; } = new();
        public PcWaterCoolant Coolant { get; set; } = new();

        public bool HasTemperatureSensor { get; set; }
        public bool HasFlowSensor { get; set; }
        public bool HasLeakSensor { get; set; }
        public string SensorsDescription { get; set; } = "";

        public bool HasFillPort { get; set; }
        public bool HasDrainValve { get; set; }

        public string Notes { get; set; } = "";
    }

    internal class PcWaterLoopComponent
    {
        public bool IsUsed { get; set; }

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string FittingsDescription { get; set; } = "";
    }

    internal class PcWaterReservoir
    {
        public bool IsInstalled { get; set; }

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string Capacity { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";

        public string RgbType { get; set; } = "";
    }

    internal class PcWaterPump
    {
        public bool IsInstalled { get; set; }

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string PumpType { get; set; } = "";
        // D5 / DDC / Other

        public string OtherPumpType { get; set; } = "";

        public string Rpm { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";

        public string RgbType { get; set; } = "";
    }

    internal class PcWaterRadiator
    {
        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string Size { get; set; } = "";
        // 120 / 140 / 240 / 280 / 360 / 420 / 480 / 560 / Other

        public string OtherSize { get; set; } = "";

        public string Thickness { get; set; } = "";
        // Slim / Standard / Thick / Other

        public string OtherThickness { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";
    }

    internal class PcWaterTubing
    {
        public string TubingType { get; set; } = "";
        // Soft / Hard / Mixed / Other

        public string OtherTubingType { get; set; } = "";

        public string Material { get; set; } = "";
        // PVC / PETG / Acrylic / Glass / Silicone / Other

        public string OtherMaterial { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";
    }

    internal class PcWaterFittings
    {
        public bool IsInstalled { get; set; }

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";

        public string Description { get; set; } = "";
    }

    internal class PcWaterCoolant
    {
        public bool IsInstalled { get; set; }

        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";

        public string PrimaryColor { get; set; } = "";
        public string SecondaryColor { get; set; } = "";

        public bool IsOpaque { get; set; }
    }
}