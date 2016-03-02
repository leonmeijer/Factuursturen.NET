using Newtonsoft.Json;

namespace LVMS.FactuurSturen.Model
{
    public class Taxes
    {
        public int Percentage { get; set; }
        [JsonConverter(typeof(UpperCaseStringEnumConverter))]
        public TaxRates Type { get; set; }
        public bool Default { get; set; }
        public string Country { get; set; }
    }

    public enum TaxRates
    {
        /// <summary>
        /// No taxes
        /// </summary>
        N,
        /// <summary>
        /// High taxes
        /// </summary>
        H,
        /// <summary>
        /// Low taxes
        /// </summary>
        L,
        /// <summary>
        /// Zero tax rate
        /// </summary>
        Z
    }
}