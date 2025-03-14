using SommusProject.Models;

namespace SommusProject.Services;

public interface IAlertDengueService
{
    Task<IEnumerable<AlertDengue>?> GetAlertsDengue();
    Task<IEnumerable<AlertDengue>?> GetNewAlertsDengue();
}