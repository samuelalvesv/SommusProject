using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SommusProject.Models;
using SommusProject.Options;

namespace SommusProject.Services;

public class AlertDengueService
{
    private readonly HttpClient _httpClient;
    private readonly AlertDengueOptions _options;
    public AlertDengueService(HttpClient httpClient, IOptions<AlertDengueOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    
    public async Task<IEnumerable<DengueAlert>?> GetDengueAlerts()
    {
        try
        {
            var (semanaInicial, semanaFinal, anoInicial, anoFinal) = CalcularPeriodo();
            var url = ConstruirUrl(semanaInicial, semanaFinal, anoInicial, anoFinal);
            
            return await ObterDados(url);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Erro ao obter dados do Alerta Dengue. Detalhes: {e.Message}");
        }
    }
    
    private (int semanaInicial, int semanaFinal, int anoInicial, int anoFinal) CalcularPeriodo()
    {
        var dataAtual = DateTime.Now;
        var dataInicial = dataAtual.AddMonths(-_options.MesesRetroativos);
        
        var semanaFinal = ISOWeek.GetWeekOfYear(dataAtual);
        var semanaInicial = semanaFinal - 26;

        if (dataAtual.Year != dataInicial.Year)
        {
            var semanasNoAnoAnterior = ISOWeek.GetWeeksInYear(dataInicial.Year);
            semanaInicial = semanasNoAnoAnterior + semanaFinal - 26;
        }

        return (semanaInicial, semanaFinal, dataInicial.Year, dataAtual.Year);
    }
    
    private string ConstruirUrl(int semanaInicial, int semanaFinal, int anoInicial, int anoFinal)
    {
        var parametros = new Dictionary<string, string>
        {
            {"geocode", _options.CodigoGeografico},
            {"disease", _options.Doenca},
            {"format", _options.Formato},
            {"ew_start", semanaInicial.ToString()},
            {"ew_end", semanaFinal.ToString()},
            {"ey_start", anoInicial.ToString()},
            {"ey_end", anoFinal.ToString()}
        };

        var consulta = string.Join("&", parametros.Select(x => $"{x.Key}={x.Value}"));
        return $"{_options.UrlBase}?{consulta}";
    }

    private async Task<IEnumerable<DengueAlert>?> ObterDados(string url)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var resposta = await _httpClient.GetAsync(url, cts.Token);

            if (!resposta.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Erro na requisição: {resposta.StatusCode} - {resposta.ReasonPhrase}");

            var conteudo = await resposta.Content.ReadAsStringAsync(cts.Token);
            
            if (string.IsNullOrWhiteSpace(conteudo))
                return [];

            return JsonSerializer.Deserialize<IEnumerable<DengueAlert>>(conteudo);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Timeout ao aguardar resposta do serviço");
        }
    }
}