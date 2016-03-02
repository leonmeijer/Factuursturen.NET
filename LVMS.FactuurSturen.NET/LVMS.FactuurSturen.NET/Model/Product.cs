namespace LVMS.FactuurSturen.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Taxes { get; set; }
        public double Priceintax { get; set; }
    }
}