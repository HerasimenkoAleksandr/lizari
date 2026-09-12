namespace lizari.Models.PetModels
{
    public class SupplierProduct
    {
        public int Id { get; set; }

        public bool Available { get; set; }

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        public int CategoryId { get; set; }

        public string Picture { get; set; } = "";

        public string VendorCode { get; set; } = "";

        public string Vendor { get; set; } = "";

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";
    }
}