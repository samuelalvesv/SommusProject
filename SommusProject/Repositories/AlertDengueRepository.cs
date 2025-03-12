using SommusProject.Data;
using SommusProject.Models;

namespace SommusProject.Repositories;

public class AlertDengueRepository
{
    private readonly AlertDengueDbContext _context;

    public AlertDengueRepository(AlertDengueDbContext context)
    {
        _context = context;
    }
}