namespace HGC.Models
{
    public class HardwareModel
    {
        public string Manufacturer { get; set; } = "";

        public string Family { get; set; } = "";

        public string Platform { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string ModelCode { get; set; } = "";

        public string Revision { get; set; } = "";

        public string Region { get; set; } = "";

        public string RegionCode { get; set; } = "";

        public string VariantCode { get; set; } = "";

        public string FullModelCode
        {
            get
            {
                string code = ModelCode;

                if (!string.IsNullOrWhiteSpace(RegionCode) &&
                    code.Contains("xx"))
                {
                    code = code.Replace("xx", RegionCode);
                }

                if (!string.IsNullOrWhiteSpace(VariantCode))
                {
                    code += VariantCode;
                }

                return code;
            }
        }
    }
}