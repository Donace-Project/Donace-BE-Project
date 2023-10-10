using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class CalendarParticipationRepository : RepositoryBase<CalendarParticipation>, ICalendarParticipationRepository
{
    public CalendarParticipationRepository(CalendarDbContext db) : base(db)
    {
    }
}
