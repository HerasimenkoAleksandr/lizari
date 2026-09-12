namespace lizari.Services.PetServices;

public interface IProductSyncService
{
    // Добавляет в базу выбранный товар по ID поставщика
    Task<(bool Success, string Message)> AddSelectedProductAsync(
        int supplierProductId);

    // Обновляет цены, наличие и остатки всех выбранных товаров
    Task<int> SyncSelectedProductsAsync();

    // Полностью обновляет информацию поставщика для одного товара
    Task<(bool Success, string Message)> RefreshSupplierProductAsync(
        int productId);
}