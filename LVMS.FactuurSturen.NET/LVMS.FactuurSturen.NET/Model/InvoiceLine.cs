using Newtonsoft.Json;

namespace LVMS.FactuurSturen.Model
{
    public class InvoiceLine
    {
        public double Amount { get; set; }
        [JsonProperty("amount_desc")]
        public string AmountDesc { get; set; }
        public string Description { get; set; }
        [JsonProperty("tax_rate")]
        public object TaxRate { get; set; }
        public double Price { get; set; }
        public double DiscountPct { get; set; }
        public double Linetotal { get; set; }
    }
}