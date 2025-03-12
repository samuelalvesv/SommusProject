using System.Globalization;
using System.Text.Json;
using SommusProject.Models;

namespace SommusProject.Services;

public class AlertDengueService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://info.dengue.mat.br/api/alertcity";

    public AlertDengueService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<DengueAlert>?> GetDengueAlerts()
    {
        var hoje = DateTime.Now;
        var mesesAtras = hoje.AddMonths(-6);
        var semanaFinal = ISOWeek.GetWeekOfYear(hoje);
        var semanaInicial = semanaFinal - 26;
        
        if (hoje.Year != mesesAtras.Year)
        {
            var semanasNoAnoAnterior = ISOWeek.GetWeeksInYear(mesesAtras.Year);
            semanaInicial = semanasNoAnoAnterior + semanaFinal - 26;
        }
        
        var parametros = new Dictionary<string, string>
        {
            {"geocode", "3106200"},
            {"disease", "dengue"},
            {"format", "json"},
            {"ew_start", semanaInicial.ToString()},
            {"ew_end", semanaFinal.ToString()},
            {"ey_start", mesesAtras.Year.ToString()},
            {"ey_end", hoje.Year.ToString()}
        };

        var query = string.Join("&", parametros.Select(x => $"{x.Key}={x.Value}"));
        var url = $"{BaseUrl}?{query}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<DengueAlert>>(content);
    }
}