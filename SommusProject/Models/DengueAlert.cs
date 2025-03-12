namespace SommusProject.Models;

public class DengueAlert
{
    private DengueAlert() { }
    
    public int SE { get; private set; }
    public int Casos { get; private set; }
    public double CasosEst { get; private set; }
    public double CasosEstMax { get; private set; }
    public double CasosEstMin { get; private set; }
    public DateTime DataIniSE { get; private set; }
    public string Id { get; private set; }
    public int Nivel { get; private set; }
    public double PInd100k { get; private set; }
    public double PRt1 { get; private set; }
    public string VersaoModelo { get; private set; }
    
    public DengueAlert(int se, int casos, double casosEst, double casosEstMax, double casosEstMin, DateTime dataIniSe, string id, int nivel, double pInd100K, double pRt1, string versaoModelo)
    {
        SE = se;
        Casos = casos;
        CasosEst = casosEst;
        CasosEstMax = casosEstMax;
        CasosEstMin = casosEstMin;
        DataIniSE = dataIniSe;
        Id = id;
        Nivel = nivel;
        PInd100k = pInd100K;
        PRt1 = pRt1;
        VersaoModelo = versaoModelo;
    }
}