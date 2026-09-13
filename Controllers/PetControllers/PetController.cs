using lizari.Data;
using lizari.Models.PetModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lizari.Controllers.PetControllers;

public class PetController : Controller
{
    private readonly DataContext _context;
    private readonly ILogger<PetController> _logger;

    public PetController(
        DataContext context,
        ILogger<PetController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // =========================
    // СПИСОК ТОВАРОВ
    // =========================

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(product => product.IsPublished)
            .OrderByDescending(product => product.Id)
            .ToListAsync();

        return View(products);
    }

    // =========================
    // ЛЕНДИНГ ТОВАРА
    // =========================

    public async Task<IActionResult> Product(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product =>
                product.Id == id &&
                product.IsPublished);

        if (product is null)
        {
            _logger.LogWarning(
                "Опубликованный товар {ProductId} не найден.",
                id);

            return NotFound();
        }

        var otherProducts = await _context.Products
            .AsNoTracking()
            .Where(item =>
                item.Id != id &&
                item.IsPublished &&
                item.Available)
            .OrderByDescending(item => item.Id)
            .Take(4)
            .ToListAsync();

        var model = new ProductLandingViewModel
        {
            Product = product,
            OtherProducts = otherProducts
        };

        return View(model);
    }
}