namespace SommusProject.Options;

public record AlertDengueOptions
{
    public string UrlBase { get; init; } = "https://info.dengue.mat.br/api/alertcity";
    public string CodigoGeografico { get; init; } = "3106200";
    public string Doenca { get; init; } = "dengue";
    public string Formato { get; init; } = "json";
    public int MesesRetroativos { get; init; } = 6;
}