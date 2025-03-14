using SommusProject.Models;

namespace SommusProject.Repositories;

public interface IAlertDengueRepository
{
    Task<IEnumerable<AlertDengue>> GetAllAlertsDengue();
    Task<IEnumerable<long>> GetAllAlertsDengueId();
    Task<bool> SalvarAlertasDengue(List<AlertDengue> alertasDengue);
    Task<AlertDengue?> ConsultarPorSemanaAno(int semanaEpidemiologica, int ano);
}