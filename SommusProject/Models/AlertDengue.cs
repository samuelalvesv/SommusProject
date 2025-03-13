using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace SommusProject.Models;

[Table("ALERTA_DENGUE")]
public class AlertDengue
{
    [JsonPropertyName("data_iniSE")]
    public long DataInicioSemanaEpidemiologicDaTimestamp { get; set; }

    [JsonIgnore]
    [Write(false)]
    public DateTime DataInicioSemanaEpidemiologica => 
        DateTimeOffset.FromUnixTimeMilliseconds(DataInicioSemanaEpidemiologicDaTimestamp).DateTime;

    [JsonPropertyName("SE")]
    public int SemanaEpidemiologica { get; set; }

    [JsonPropertyName("casos_est")]
    public double CasosEstimados { get; set; }

    [JsonPropertyName("casos_est_min")]
    public double CasosEstimadosMinimo { get; set; }

    [JsonPropertyName("casos_est_max")]
    public double CasosEstimadosMaximo { get; set; }

    [JsonPropertyName("casos")]
    public int CasosNotificados { get; set; }

    [JsonPropertyName("p_rt1")]
    public double ProbabilidadeTransmissao { get; set; }

    [JsonPropertyName("p_inc100k")]
    public double IncidenciaPor100kHabitantes { get; set; }

    [JsonPropertyName("Localidade_id")]
    public int CodigoLocalidade { get; set; }

    [JsonPropertyName("nivel")]
    public int NivelRisco { get; set; }

    [JsonPropertyName("id")]
    public long Identificador { get; set; }

    [JsonPropertyName("versao_modelo")]
    public string VersaoModelo { get; set; } = default!;

    [JsonPropertyName("tweet")]
    public string? Mensagem { get; set; }

    [JsonPropertyName("Rt")]
    public double TaxaTransmissao { get; set; }

    [JsonPropertyName("pop")]
    public double Populacao { get; set; }

    [JsonPropertyName("tempmin")]
    public double? TemperaturaMinima { get; set; }

    [JsonPropertyName("umidmax")]
    public double? UmidadeMaxima { get; set; }

    [JsonPropertyName("receptivo")]
    public int AreaReceptiva { get; set; }

    [JsonPropertyName("transmissao")]
    public int AreaTransmissao { get; set; }

    [JsonPropertyName("nivel_inc")]
    public int NivelIncidencia { get; set; }

    [JsonPropertyName("umidmed")]
    public double? UmidadeMedia { get; set; }

    [JsonPropertyName("umidmin")]
    public double? UmidadeMinima { get; set; }

    [JsonPropertyName("tempmed")]
    public double? TemperaturaMedia { get; set; }

    [JsonPropertyName("tempmax")]
    public double? TemperaturaMaxima { get; set; }

    [JsonPropertyName("casprov")]
    public int CasosProvaveis { get; set; }

    [JsonPropertyName("casprov_est")]
    public double? CasosProvaveisEstimados { get; set; }

    [JsonPropertyName("casprov_est_min")]
    public double? CasosProvaveisEstimadosMinimo { get; set; }

    [JsonPropertyName("casprov_est_max")]
    public double? CasosProvaveisEstimadosMaximo { get; set; }

    [JsonPropertyName("casconf")]
    public int? CasosConfirmados { get; set; }

    [JsonPropertyName("notif_accum_year")]
    public int NotificacoesAcumuladasAno { get; set; }
}