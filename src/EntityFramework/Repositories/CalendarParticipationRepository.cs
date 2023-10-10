using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using System.Linq.Expressions;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class CalendarParticipationRepository : RepositoryBase<CalendarParticipation>, ICalendarParticipationRepository
{
    public CalendarParticipationRepository(CalendarDbContext db, IUserProvider userProvider) : base(db, userProvider)
    {
    }    
}
