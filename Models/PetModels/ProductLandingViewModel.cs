using lizari.Entities;

namespace lizari.Models.PetModels;

public class ProductLandingViewModel
{
    public ProductEntity Product { get; set; } = null!;

    public IReadOnlyList<ProductEntity> OtherProducts { get; set; } =
        Array.Empty<ProductEntity>();
}