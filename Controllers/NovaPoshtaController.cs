using lizari.Services.NovaPoshta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lizari.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/nova-poshta")]
public class NovaPoshtaController : ControllerBase
{
    private readonly INovaPoshtaService _novaPoshtaService;
    private readonly ILogger<NovaPoshtaController> _logger;

    public NovaPoshtaController(
        INovaPoshtaService novaPoshtaService,
        ILogger<NovaPoshtaController> logger)
    {
        _novaPoshtaService = novaPoshtaService;
        _logger = logger;
    }


    // =========================
    // ПОИСК ГОРОДОВ
    // =========================

    [HttpGet("cities")]
    public async Task<IActionResult> SearchCities(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        query = query?.Trim() ?? string.Empty;

        if (query.Length < 2)
        {
            return Ok(Array.Empty<object>());
        }

        try
        {
            var cities =
                await _novaPoshtaService.SearchCitiesAsync(
                    query,
                    cancellationToken);

            var result = cities.Select(city => new
            {
                city.Ref,
                city.Description,
                city.AreaDescription,
                city.SettlementTypeDescription
            });

            return Ok(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ошибка поиска городов Новой Почты.");

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    message = exception.Message
                });
        }
    }


    // =========================
    // ПОЛУЧЕНИЕ ОТДЕЛЕНИЙ
    // =========================

    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses(
        [FromQuery] string cityRef,
        [FromQuery] string? query,
        CancellationToken cancellationToken)
    {
        cityRef = cityRef?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cityRef))
        {
            return BadRequest(new
            {
                message = "Не вказано місто."
            });
        }

        try
        {
            var warehouses =
                await _novaPoshtaService.GetWarehousesAsync(
                    cityRef,
                    query,
                    cancellationToken);

            var result = warehouses.Select(warehouse => new
            {
                warehouse.Ref,
                warehouse.Description,
                warehouse.Number,
                warehouse.ShortAddress,
                warehouse.TypeOfWarehouse
            });

            return Ok(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ошибка загрузки отделений Новой Почты.");

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    message =
                        "Не вдалося завантажити відділення."
                });
        }
    }
}