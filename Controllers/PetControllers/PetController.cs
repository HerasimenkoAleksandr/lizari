using lizari.Services.PetServices;
using Microsoft.AspNetCore.Mvc;

namespace lizari.Controllers.PetControllers
{
    public class PetController : Controller
    {
        private readonly IProductFeedService _productFeedService;
        private readonly ILogger<PetController> _logger;

        public PetController(IProductFeedService productFeedService, ILogger<PetController> logger)
        {
            _productFeedService = productFeedService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productFeedService.GetProductsAsync();
            var categories = await _productFeedService.GetCategoriesAsync();

            // Передаём список продуктов в представление
            return View(products);
        }

        public async Task<IActionResult> Product(int id)
        {
            var products = await _productFeedService.GetProductsAsync();

            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = id;

            return View(products);
        }
    }
}
