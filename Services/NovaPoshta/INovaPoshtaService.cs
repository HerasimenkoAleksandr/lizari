using lizari.Services.NovaPoshta.Models;

namespace lizari.Services.NovaPoshta;

public interface INovaPoshtaService
{
    Task<IReadOnlyList<NovaPoshtaCity>> SearchCitiesAsync(
        string searchText,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NovaPoshtaWarehouse>> GetWarehousesAsync(
        string cityRef,
        string? searchText = null,
        CancellationToken cancellationToken = default);
}