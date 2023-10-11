using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.Interfaces.Services;

namespace Donace_BE_Project.EntityFramework;
public class UnitOfWork : IUnitOfWork
{
    private readonly CalendarDbContext _context;
    private readonly CustomerDbContext _customerContext;
    public UnitOfWork(CalendarDbContext dbContext, 
                      CustomerDbContext customerContext)
    {
        _context = dbContext;
        _customerContext = customerContext;
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

    public async Task SaveChangeCusAsync()
    {
        await _customerContext.SaveChangesAsync();
    }
}
