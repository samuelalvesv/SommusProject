using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using SommusProject.Models;
using SommusProject.Options;
using SommusProject.Repositories;
using SommusProject.Services;

namespace SommusProject.Test;

public class AlertDengueServiceTests
{
    private readonly Mock<IAlertDengueRepository> _repositoryMock;
    private readonly AlertDengueOptions _options;
    private readonly MemoryCache _memoryCache;

    public AlertDengueServiceTests()
    {
        _repositoryMock = new Mock<IAlertDengueRepository>();
        
        _options = new AlertDengueOptions
        {
            UrlBase = "https://api.teste.com/alertas",
            CodigoGeografico = "12345",
            Doenca = "dengue",
            Formato = "json",
            MesesRetroativos = 3
        };
        
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetAlertsDengue_QuandoCacheExiste_DeveRetornarDadosDoCache()
    {
        // Arrange
        var alertasMock = new List<AlertDengue>
        {
            new() { Identificador = 1 },
            new() { Identificador = 2 }
        };
        
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dataFixa = new DateTime(2023, 10, 15);
        dateTimeProviderMock.Setup(d => d.Now).Returns(dataFixa);
        
        var cacheKey = $"alertas_dengue_{dataFixa:yyyy-MM-dd}";
        _memoryCache.Set(cacheKey, alertasMock);
        
        var optionsMock = new Mock<IOptions<AlertDengueOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var httpClientMock = new Mock<IHttpClientWrapper>();
        var service = new AlertDengueService(httpClientMock.Object, optionsMock.Object, _repositoryMock.Object, _memoryCache, dateTimeProviderMock.Object);
        
        // Act
        var resultado = (await service.GetAlertsDengue())?.ToList();
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.Equal(1, resultado.First().Identificador);
    }

    [Fact]
    public async Task GetAlertsDengue_QuandoCacheVazio_DeveConsultarAPI()
    {
        // Arrange
        var alertasMock = new List<AlertDengue>
        {
            new() { Identificador = 1 },
            new() { Identificador = 2 }
        };
        
        var jsonResponse = JsonSerializer.Serialize(alertasMock);
        
        var httpClientMock = new Mock<IHttpClientWrapper>();
        httpClientMock
            .Setup(h => h.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });
        
        var optionsMock = new Mock<IOptions<AlertDengueOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dataFixa = new DateTime(2023, 10, 15);
        dateTimeProviderMock.Setup(d => d.Now).Returns(dataFixa);
        
        var service = new AlertDengueService(httpClientMock.Object, optionsMock.Object, _repositoryMock.Object, _memoryCache, dateTimeProviderMock.Object);
        
        // Act
        var resultado = await service.GetAlertsDengue();
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task GetAlertsDengue_QuandoConsultaAPI_DevePreencherCache()
    {
        // Arrange
        var alertasMock = new List<AlertDengue>
        {
            new AlertDengue { Identificador = 1 },
            new AlertDengue { Identificador = 2 }
        };

        var jsonResponse = JsonSerializer.Serialize(alertasMock);

        var httpClientMock = new Mock<IHttpClientWrapper>();
        httpClientMock
            .Setup(h => h.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
    
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dataFixa = new DateTime(2023, 10, 15);
        dateTimeProviderMock.Setup(d => d.Now).Returns(dataFixa);
        
        var cacheKey = $"alertas_dengue_{dataFixa:yyyy-MM-dd}";
        var optionsMock = new Mock<IOptions<AlertDengueOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);

        var service = new AlertDengueService(httpClientMock.Object, optionsMock.Object, _repositoryMock.Object, memoryCache, dateTimeProviderMock.Object);

        // Act
        var resultado = await service.GetAlertsDengue();

        // Assert
        // Verifica se os dados retornados são os esperados
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    
        // Verifica se o cache foi preenchido
        Assert.True(memoryCache.TryGetValue(cacheKey, out IEnumerable<AlertDengue>? cachedAlerts));
        Assert.NotNull(cachedAlerts);
        Assert.Equal(2, cachedAlerts.Count());
    }
    
    [Fact]
    public async Task GetAlertsDengue_QuandoAPIRetornaErro_DeveLancarExcecao()
    {
        // Arrange
        var httpClientMock = new Mock<IHttpClientWrapper>();
        httpClientMock
            .Setup(h => h.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Requisição inválida"
            });
        
        var optionsMock = new Mock<IOptions<AlertDengueOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dataFixa = new DateTime(2023, 10, 15);
        dateTimeProviderMock.Setup(d => d.Now).Returns(dataFixa);
        
        var service = new AlertDengueService(httpClientMock.Object, optionsMock.Object, _repositoryMock.Object, _memoryCache, dateTimeProviderMock.Object);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAlertsDengue());
        Assert.Contains("Erro ao obter dados do Alerta Dengue", exception.Message);
    }

    [Fact]
    public async Task GetAlertsDengue_QuandoAPIRetornaRespostaVazia_DeveRetornarListaVazia()
    {
        // Arrange
        var httpClientMock = new Mock<IHttpClientWrapper>();
        httpClientMock
            .Setup(h => h.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
        
        var optionsMock = new Mock<IOptions<AlertDengueOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dataFixa = new DateTime(2023, 10, 15);
        dateTimeProviderMock.Setup(d => d.Now).Returns(dataFixa);
        
        var service = new AlertDengueService(httpClientMock.Object, optionsMock.Object, _repositoryMock.Object, _memoryCache, dateTimeProviderMock.Object);
        
        // Act
        var resultado = await service.GetAlertsDengue();
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }
    
    [Fact]
    public async Task GetAlertsDengue_QuandoTimeoutNaAPI_DeveLancarExcecao()
    {
        // Arrange
        var httpClientMock = new Mock<IHttpClientWrapper>();
        httpClientMock
            .Setup(h => h.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        var optionsMock = new Mock<IOptions<AlertDengueOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);
        
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dataFixa = new DateTime(2023, 10, 15);
        dateTimeProviderMock.Setup(d => d.Now).Returns(dataFixa);
        
        var service = new AlertDengueService(httpClientMock.Object, optionsMock.Object, _repositoryMock.Object, _memoryCache, dateTimeProviderMock.Object);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAlertsDengue());
        Assert.Contains("Erro ao obter dados do Alerta Dengue", exception.Message);
    }
}