namespace lizari.Services.NovaPoshta;

public class NovaPoshtaOptions
{
    public const string SectionName = "NovaPoshta";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiUrl { get; set; } =
        "https://api.novaposhta.ua/v2.0/json/";
}