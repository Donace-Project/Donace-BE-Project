namespace Donace_BE_Project.Interfaces.Services;

public interface IUnitOfWork
{
    Task SaveChangeAsync();
    Task RollbackAsync();
    Task CommitAsync();
    Task SaveChangeCusAsync();
}