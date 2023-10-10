using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class CalendarRepository : RepositoryBase<Calendar>, ICalendarRepository
{
    public CalendarRepository(AppDbContext db) : base(db)
    {
    }
}
