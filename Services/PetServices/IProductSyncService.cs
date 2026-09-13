namespace lizari.Services.PetServices;

public interface IProductSyncService
{
    // Добавляет товар по ID или артикулу поставщика
    Task<(bool Success, string Message)> AddSelectedProductAsync(
        string searchValue);

    // Обновляет цены, наличие и остатки всех выбранных товаров
    Task<int> SyncSelectedProductsAsync();

    // Полностью обновляет информацию поставщика для одного товара
    Task<(bool Success, string Message)> RefreshSupplierProductAsync(
        int productId);
}

