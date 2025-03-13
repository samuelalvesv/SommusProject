using SommusProject.Models;

namespace SommusProject.Repositories;

public interface IAlertDengueRepository
{
    Task<bool> SalvarAlertasDengue(List<AlertDengue> alertasDengue);
    Task<AlertDengue?> ConsultarPorSemanaAno(int semanaEpidemiologica, int ano);
}