using HGC.Models;
using System.Collections.Generic;

namespace HGC.Data
{
    public static class HardwareBoardRevisionDatabase
    {
        public static List<HardwareBoardRevision> GetBoardRevisions()
        {
            return new()
            {
                // Xbox 360 Fat

                B("Microsoft", "Xbox 360", "Xenon"),
                B("Microsoft", "Xbox 360", "Zephyr"),
                B("Microsoft", "Xbox 360", "Falcon"),
                B("Microsoft", "Xbox 360", "Opus"),
                B("Microsoft", "Xbox 360", "Jasper"),

                // Xbox 360 Slim

                B("Microsoft", "Xbox 360", "Trinity"),
                B("Microsoft", "Xbox 360", "Corona"),

                // Xbox 360 E

                B("Microsoft", "Xbox 360", "Winchester")
            };
        }

        private static HardwareBoardRevision B(
            string manufacturer,
            string platform,
            string revisionName)
        {
            return new HardwareBoardRevision
            {
                Manufacturer = manufacturer,
                Platform = platform,
                RevisionName = revisionName
            };
        }
    }
}