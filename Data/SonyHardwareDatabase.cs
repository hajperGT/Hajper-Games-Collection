using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class SonyHardwareDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new()
            {
                
// =========================
// SONY - PLAYSTATION / PS ONE
// =========================

// Standard PlayStation

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-1000", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-1001", Revision = "", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-1002", Revision = "", Region = "PAL" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-3000", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-3500", Revision = "", Region = "NTSC-J" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5000", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5003", Revision = "", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5500", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5501", Revision = "", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5502", Revision = "", Region = "PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5503", Revision = "", Region = "Asia" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5552", Revision = "", Region = "PAL" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-5903", Revision = "", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7000", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7001", Revision = "", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7002", Revision = "", Region = "PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7003", Revision = "", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7500", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7501", Revision = "", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7502", Revision = "", Region = "PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-7503", Revision = "", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-9000", Revision = "", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-9001", Revision = "", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-9002", Revision = "", Region = "PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "PlayStation", ModelCode = "SCPH-9003", Revision = "", Region = "Asia" },

// PS One

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PS One", DisplayName = "PS One", ModelCode = "SCPH-100", Revision = "PS One", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PS One", DisplayName = "PS One", ModelCode = "SCPH-101", Revision = "PS One", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PS One", DisplayName = "PS One", ModelCode = "SCPH-102", Revision = "PS One", Region = "PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PS One", DisplayName = "PS One", ModelCode = "SCPH-103", Revision = "PS One", Region = "Asia" },

// Custom

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation", DisplayName = "Other / Custom", ModelCode = "Other", Revision = "Other", Region = "Other" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PS One", DisplayName = "Other / Custom", ModelCode = "Other", Revision = "Other", Region = "Other" },

// =========================
// SONY - PLAYSTATION 2
// =========================

// FAT

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-10000", Revision = "Fat", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-15000", Revision = "Fat", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-18000", Revision = "Fat", Region = "NTSC-J" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-30000", Revision = "Fat", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-30001", Revision = "Fat", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-30002", Revision = "Fat", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-30003", Revision = "Fat", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-30004", Revision = "Fat", Region = "Europe / PAL" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-35000", Revision = "Fat", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-35001", Revision = "Fat", Region = "NTSC-U" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-37000", Revision = "Fat", Region = "NTSC-J" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-39000", Revision = "Fat", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-39001", Revision = "Fat", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-39002", Revision = "Fat", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-39003", Revision = "Fat", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-39004", Revision = "Fat", Region = "Europe / PAL" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-50000", Revision = "Fat", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-50001", Revision = "Fat", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-50002", Revision = "Fat", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-50003", Revision = "Fat", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2", ModelCode = "SCPH-50004", Revision = "Fat", Region = "Europe / PAL" },

// SLIM

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70000", Revision = "Slim", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70001", Revision = "Slim", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70002", Revision = "Slim", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70003", Revision = "Slim", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70004", Revision = "Slim", Region = "Europe / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70006", Revision = "Slim", Region = "Asia" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70011", Revision = "Slim", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-70012", Revision = "Slim", Region = "NTSC-U" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-75000", Revision = "Slim", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-75001", Revision = "Slim", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-75002", Revision = "Slim", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-75003", Revision = "Slim", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-75004", Revision = "Slim", Region = "Europe / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-75006", Revision = "Slim", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-77000", Revision = "Slim", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-77001", Revision = "Slim", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-77002", Revision = "Slim", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-77003", Revision = "Slim", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-77004", Revision = "Slim", Region = "Europe / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-77006", Revision = "Slim", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-79000", Revision = "Slim", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-79001", Revision = "Slim", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-79002", Revision = "Slim", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-79003", Revision = "Slim", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-79004", Revision = "Slim", Region = "Europe / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-79006", Revision = "Slim", Region = "Asia" },

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-90000", Revision = "Slim", Region = "NTSC-J" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-90001", Revision = "Slim", Region = "NTSC-U" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-90002", Revision = "Slim", Region = "Oceania / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-90003", Revision = "Slim", Region = "UK / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-90004", Revision = "Slim", Region = "Europe / PAL" },
new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "PlayStation 2 Slim", ModelCode = "SCPH-90006", Revision = "Slim", Region = "Asia" },

// Custom

new HardwareModel { Manufacturer = "Sony", Family = "PlayStation", Platform = "PlayStation 2", DisplayName = "Other / Custom", ModelCode = "Other", Revision = "Other", Region = "Other" },

// =========================
// SONY - PLAYSTATION 3
// =========================

// FAT
// RegionCode uzupełniamy później z HardwareRegionDatabase.
// Dla modeli FAT nie używamy VariantCode.

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHAxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHBxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHCxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHExx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHGxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHHxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHJxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHKxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHLxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHMxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHPxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3",
    ModelCode = "CECHQxx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// SLIM
// RegionCode + VariantCode składają pełny model, np.
// CECH-25xx + 04 + B = CECH-2504B

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Slim",
    ModelCode = "CECH-20xx",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Slim",
    ModelCode = "CECH-21xx",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Slim",
    ModelCode = "CECH-25xx",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Slim",
    ModelCode = "CECH-30xx",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// SUPER SLIM
// RegionCode + VariantCode składają pełny model, np.
// CECH-42xx + 04 + C = CECH-4204C

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Super Slim",
    ModelCode = "CECH-40xx",
    Revision = "Super Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Super Slim",
    ModelCode = "CECH-42xx",
    Revision = "Super Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "PlayStation 3 Super Slim",
    ModelCode = "CECH-43xx",
    Revision = "Super Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// Custom

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 3",
    DisplayName = "Other / Custom",
    ModelCode = "Other",
    Revision = "Other",
    Region = "Other",
    RegionCode = "",
    VariantCode = ""
},

// =========================
// SONY - PLAYSTATION 4
// =========================

// STANDARD

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4",
    DisplayName = "PlayStation 4",
    ModelCode = "CUH-10xx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4",
    DisplayName = "PlayStation 4",
    ModelCode = "CUH-11xx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4",
    DisplayName = "PlayStation 4",
    ModelCode = "CUH-12xx",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// SLIM

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4 Slim",
    DisplayName = "PlayStation 4 Slim",
    ModelCode = "CUH-20xx",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4 Slim",
    DisplayName = "PlayStation 4 Slim",
    ModelCode = "CUH-21xx",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// PRO

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4 Pro",
    DisplayName = "PlayStation 4 Pro",
    ModelCode = "CUH-70xx",
    Revision = "Pro",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4 Pro",
    DisplayName = "PlayStation 4 Pro",
    ModelCode = "CUH-71xx",
    Revision = "Pro",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4 Pro",
    DisplayName = "PlayStation 4 Pro",
    ModelCode = "CUH-72xx",
    Revision = "Pro",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// Custom

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 4",
    DisplayName = "Other / Custom",
    ModelCode = "Other",
    Revision = "Other",
    Region = "Other",
    RegionCode = "",
    VariantCode = ""
},

// =========================
// SONY - PLAYSTATION 5
// =========================

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5",
    DisplayName = "PlayStation 5",
    ModelCode = "CFI-10xxA",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5 Digital",
    DisplayName = "PlayStation 5 Digital",
    ModelCode = "CFI-10xxB",
    Revision = "Digital",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5",
    DisplayName = "PlayStation 5",
    ModelCode = "CFI-11xxA",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5 Digital",
    DisplayName = "PlayStation 5 Digital",
    ModelCode = "CFI-11xxB",
    Revision = "Digital",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5",
    DisplayName = "PlayStation 5",
    ModelCode = "CFI-12xxA",
    Revision = "Fat",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5 Digital",
    DisplayName = "PlayStation 5 Digital",
    ModelCode = "CFI-12xxB",
    Revision = "Digital",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5 Slim",
    DisplayName = "PlayStation 5 Slim",
    ModelCode = "CFI-20xxA",
    Revision = "Slim",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5 Slim",
    DisplayName = "PlayStation 5 Slim Digital",
    ModelCode = "CFI-20xxB",
    Revision = "Slim Digital",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5 Pro",
    DisplayName = "PlayStation 5 Pro",
    ModelCode = "CFI-70xx",
    Revision = "Pro",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation",
    Platform = "PlayStation 5",
    DisplayName = "Other / Custom",
    ModelCode = "Other",
    Revision = "Other",
    Region = "Other",
    RegionCode = "",
    VariantCode = ""
},

// =========================
// SONY - PSP
// =========================

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PSP",
    Platform = "PSP",
    DisplayName = "PSP",
    ModelCode = "PSP-10xx",
    Revision = "1000",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PSP",
    Platform = "PSP",
    DisplayName = "PSP",
    ModelCode = "PSP-20xx",
    Revision = "2000",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PSP",
    Platform = "PSP",
    DisplayName = "PSP",
    ModelCode = "PSP-30xx",
    Revision = "3000",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PSP",
    Platform = "PSP Go",
    DisplayName = "PSP Go",
    ModelCode = "PSP-N10xx",
    Revision = "Go",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PSP",
    Platform = "PSP Street",
    DisplayName = "PSP Street",
    ModelCode = "PSP-E10xx",
    Revision = "Street",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PSP",
    Platform = "PSP",
    DisplayName = "Other / Custom",
    ModelCode = "Other",
    Revision = "Other",
    Region = "Other",
    RegionCode = "",
    VariantCode = ""
},

// =========================
// SONY - PLAYSTATION VITA
// =========================

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation Vita",
    Platform = "PlayStation Vita",
    DisplayName = "PlayStation Vita",
    ModelCode = "PCH-10xx",
    Revision = "1000",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation Vita",
    Platform = "PlayStation Vita",
    DisplayName = "PlayStation Vita",
    ModelCode = "PCH-11xx",
    Revision = "1000",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation Vita",
    Platform = "PlayStation Vita",
    DisplayName = "PlayStation Vita",
    ModelCode = "PCH-20xx",
    Revision = "2000",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// =========================
// SONY - PLAYSTATION TV
// =========================

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation Vita",
    Platform = "PlayStation TV",
    DisplayName = "PlayStation TV",
    ModelCode = "VTE-10xx",
    Revision = "TV",
    Region = "Multiple",
    RegionCode = "xx",
    VariantCode = ""
},

// Custom

new HardwareModel
{
    Manufacturer = "Sony",
    Family = "PlayStation Vita",
    Platform = "PlayStation Vita",
    DisplayName = "Other / Custom",
    ModelCode = "Other",
    Revision = "Other",
    Region = "Other",
    RegionCode = "",
    VariantCode = ""
},
            };
        }
    }
}