﻿using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class CalendarParticipationRepository : RepositoryBase<CalendarParticipation>, ICalendarParticipationRepository
{
    public CalendarParticipationRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<CalendarParticipation> FindAsync(Expression<Func<CalendarParticipation, bool>> predicate)
    {
        return await _dbSet.FindAsync(predicate);
    }
}
