using Donace_BE_Project.Interfaces;

namespace Donace_BE_Project.EntityFramework;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task SaveChangeAsync()
    {
        await _context.SaveChangesAsync();
    }
}
