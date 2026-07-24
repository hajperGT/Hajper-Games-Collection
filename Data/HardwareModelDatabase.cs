using HGC.Models;
using System.Collections.Generic;
using System.Linq;

namespace HGC.Data
{
    public static class HardwareModelDatabase
    {
        public static List<HardwareModel> GetModels()
        {
            return new List<HardwareModel>()
                .Concat(SonyHardwareDatabase.GetModels())
                .Concat(MicrosoftHardwareDatabase.GetModels())
                .Concat(NintendoHardwareDatabase.GetModels())
                .Concat(SegaHardwareDatabase.GetModels())
                .Concat(AtariHardwareDatabase.GetModels())
                .Concat(CommodoreHardwareDatabase.GetModels())
                .Concat(CommodoreHardwareDatabase.GetModels())
                .Concat(AmigaHardwareDatabase.GetModels())
                .Concat(OtherHardwareDatabase.GetModels())
                .Concat(DeveloperHardwareDatabase.GetModels())
                .ToList();
        }
    }
}