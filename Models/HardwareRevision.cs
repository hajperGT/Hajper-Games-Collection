using HGC.Models;
using System.Collections.Generic;

namespace HGC.Models
{
    public class HardwareRevision
    {
        public string Manufacturer { get; set; } = "";

        public string Family { get; set; } = "";

        public string Platform { get; set; } = "";

        public string Revision { get; set; } = "";
    }
}
