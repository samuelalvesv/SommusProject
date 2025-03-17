using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SommusProject.Models;
using SommusProject.Options;
using SommusProject.Repositories;

namespace SommusProject.Services;

public class AlertDengueService : IAlertDengueService
{
    private readonly IHttpClientWrapper _httpClient;
    private readonly AlertDengueOptions _options;
    private readonly IAlertDengueRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AlertDengueService(
        IHttpClientWrapper httpClient, 
        IOptions<AlertDengueOptions> options,
        IAlertDengueRepository repository,
        IMemoryCache cache,
        IDateTimeProvider dateTimeProvider
        )
    {
        _httpClient = httpClient;
        _options = options.Value;
        _repository = repository;
        _cache = cache;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<IEnumerable<AlertDengue>?> GetAlertsDengue()
    {
        try
        {
            string cacheKey = $"alertas_dengue_{_dateTimeProvider.Now:yyyy-MM-dd}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<AlertDengue>? cachedAlerts))
                return cachedAlerts;

            var (semanaInicial, semanaFinal, anoInicial, anoFinal) = CalcularPeriodo();
            var url = ConstruirUrl(semanaInicial, semanaFinal, anoInicial, anoFinal);
            
            var alerts = (await ObterDados(url))?.ToList();
            _cache.Set(cacheKey, alerts, TimeSpan.FromHours(6));
            return alerts;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Erro ao obter dados do Alerta Dengue. Detalhes: {e.Message}");
        }
    }

    public async Task<IEnumerable<AlertDengue>?> GetNewAlertsDengue()
    {
        try
        {
            var alerts = (await GetAlertsDengue())?.ToList();
            
            var ids = (await _repository.GetAllAlertsDengueId()).ToList();
            
            if (ids.Any() == false)
                return alerts;

            return alerts?.Where(a => !ids.Contains(a.Identificador));
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Erro ao obter dados do Alerta Dengue. Detalhes: {e.Message}");
        }
    }
    
    private (int semanaInicial, int semanaFinal, int anoInicial, int anoFinal) CalcularPeriodo()
    {
        var dataAtual = _dateTimeProvider.Now;
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

    private async Task<IEnumerable<AlertDengue>?> ObterDados(string url)
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

            return JsonSerializer.Deserialize<IEnumerable<AlertDengue>>(conteudo);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Timeout ao aguardar resposta do serviço");
        }
    }
}