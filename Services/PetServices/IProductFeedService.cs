using lizari.Models.PetModels;

namespace lizari.Services.PetServices

{
    public interface IProductFeedService
    {
        Task<List<SupplierProduct>> GetProductsAsync();

        Task<List<Category>> GetCategoriesAsync();
    }
}
