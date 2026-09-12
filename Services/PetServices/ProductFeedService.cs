using System.Xml.Linq;
using lizari.Models.PetModels;

namespace lizari.Services.PetServices
{
    public class ProductFeedService : IProductFeedService
    {
        private readonly HttpClient _httpClient;

        

        private const string FeedUrl =
            "https://basmati.com.ua/zoobaza_prom.php";

        public ProductFeedService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        private async Task<XDocument> GetDocumentAsync()
        {
            string xml = await _httpClient.GetStringAsync(FeedUrl);

            return XDocument.Parse(xml);
        }


        public async Task<List<SupplierProduct>> GetProductsAsync()
        {
            XDocument document = await GetDocumentAsync();

            List<SupplierProduct> products = document
                .Descendants("offer")
                .Select(offer => new SupplierProduct
                {
                    Id =
                        (int?)offer.Attribute("id") ?? 0,

                    Available =
                        (bool?)offer.Attribute("available") ?? false,

                    Price =
                        (decimal?)offer.Element("price") ?? 0,

                    QuantityInStock =
                        (int?)offer.Element("quantity_in_stock") ?? 0,

                    CategoryId =
                        (int?)offer.Element("categoryId") ?? 0,

                    Picture =
                        (string?)offer.Element("picture") ?? "",

                    VendorCode =
                        (string?)offer.Element("vendorCode") ?? "",

                    Vendor =
                        (string?)offer.Element("vendor") ?? "",

                    Name =
                        ((string?)offer.Element("name") ?? "").Trim(),

                    Description =
                        ((string?)offer.Element("description") ?? "").Trim()
                })
                .ToList();

            return products;
        }


        public async Task<List<Category>> GetCategoriesAsync()
        {
            XDocument document = await GetDocumentAsync();

            List<Category> categories = document
                .Descendants("category")
                .Select(category => new Category
                {
                    Id =
                        (int?)category.Attribute("id") ?? 0,

                    ParentId =
                        (int?)category.Attribute("parentId"),

                    Name =
                        category.Value.Trim()
                })
                .ToList();

            return categories;
        }
    }
}