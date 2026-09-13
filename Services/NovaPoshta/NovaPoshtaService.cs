using System.Net.Http.Json;
using lizari.Services.NovaPoshta.Models;
using Microsoft.Extensions.Options;

namespace lizari.Services.NovaPoshta;

public class NovaPoshtaService : INovaPoshtaService
{
    private readonly HttpClient _httpClient;
    private readonly NovaPoshtaOptions _options;
    private readonly ILogger<NovaPoshtaService> _logger;

    public NovaPoshtaService(
        HttpClient httpClient,
        IOptions<NovaPoshtaOptions> options,
        ILogger<NovaPoshtaService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NovaPoshtaCity>>
        SearchCitiesAsync(
            string searchText,
            CancellationToken cancellationToken = default)
    {
        searchText = searchText?.Trim() ?? string.Empty;

        if (searchText.Length < 2)
        {
            return Array.Empty<NovaPoshtaCity>();
        }

        var request = new
        {
            apiKey = _options.ApiKey,
            modelName = "Address",
            calledMethod = "getCities",
            methodProperties = new
            {
                FindByString = searchText,
                Limit = "20",
                Page = "1"
            }
        };

        var response =
            await SendRequestAsync<NovaPoshtaCity>(
                request,
                cancellationToken);

        return response.Data;
    }

    public async Task<IReadOnlyList<NovaPoshtaWarehouse>>
        GetWarehousesAsync(
            string cityRef,
            string? searchText = null,
            CancellationToken cancellationToken = default)
    {
        cityRef = cityRef?.Trim() ?? string.Empty;
        searchText = searchText?.Trim();

        if (string.IsNullOrWhiteSpace(cityRef))
        {
            return Array.Empty<NovaPoshtaWarehouse>();
        }

        var request = new
        {
            apiKey = _options.ApiKey,
            modelName = "AddressGeneral",
            calledMethod = "getWarehouses",
            methodProperties = new
            {
                CityRef = cityRef,
                FindByString = searchText ?? string.Empty,
                Limit = "100",
                Page = "1"
            }
        };

        var response =
            await SendRequestAsync<NovaPoshtaWarehouse>(
                request,
                cancellationToken);

        return response.Data;
    }

    private async Task<NovaPoshtaResponse<T>>
        SendRequestAsync<T>(
            object request,
            CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "API-ключ Нової Пошти не налаштований.");
        }

        using var httpResponse =
            await _httpClient.PostAsJsonAsync(
                _options.ApiUrl,
                request,
                cancellationToken);

        httpResponse.EnsureSuccessStatusCode();

        var response =
            await httpResponse.Content
                .ReadFromJsonAsync<NovaPoshtaResponse<T>>(
                    cancellationToken: cancellationToken);

        if (response is null)
        {
            throw new InvalidOperationException(
                "Нова Пошта повернула порожню відповідь.");
        }

        if (!response.Success)
        {
            string errors = response.Errors.Count > 0
                ? string.Join("; ", response.Errors)
                : "Невідома помилка API.";

            _logger.LogWarning(
                "Ошибка API Новой Почты: {Errors}",
                errors);

            throw new InvalidOperationException(errors);
        }

        return response;
    }
}