using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.Interfaces.Services;

namespace Donace_BE_Project.EntityFramework;
public class UnitOfWork : IUnitOfWork
{
    private readonly CalendarDbContext _context;

    public UnitOfWork(CalendarDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task CommitAsync()
    {
        using var transaction = _context.Database.BeginTransaction();
        await transaction.CommitAsync();
    }
    public async Task RollbackAsync()
    {
        using var transaction = _context.Database.BeginTransaction();
        await transaction.RollbackAsync();
    }
    public async Task SaveChangeAsync()
    {
        await _context.SaveChangesAsync();
    }
}
