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
        public double TaxRate { get; set; }
        public double Price { get; set; }
        public double? DiscountPct { get; set; }
        public double LineTotal { get; set; }

        public InvoiceLine()
        {
            
        }

        public InvoiceLine(double amount, string description, double taxRate, double price) : this(amount, null, description,
            taxRate, price, null)
        {
            
        }

        public InvoiceLine(double amount, string amountDesc, string description, double taxRate, double price, double? discountPct)
        {
            Amount = amount;
            AmountDesc = amountDesc;
            Description = description;
            TaxRate = taxRate;
            Price = price;
            DiscountPct = discountPct;
        }
    }
}