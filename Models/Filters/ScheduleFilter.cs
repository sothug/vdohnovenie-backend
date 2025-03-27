using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class ScheduleFilter : IFilter<Schedule>
{
    public int? CoachId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public IQueryable<Schedule> Apply(IQueryable<Schedule> query, ClaimsPrincipal user)
    {
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (userRole == "Coach")
            query = query.Where(s => s.CoachId == userId);
        // else if (userRole == "Parent")
        //     query = query.Where(s => s.Attendees.Any(a => a.ParentId == userId));
        // else if (userRole == "Child")
        //     query = query.Where(s => s.Attendees.Any(a => a.Id == userId));

        if (CoachId.HasValue) query = query.Where(s => s.CoachId == CoachId);
        if (StartDate.HasValue) query = query.Where(s => s.StartTime >= StartDate);
        if (EndDate.HasValue) query = query.Where(s => s.EndTime <= EndDate);

        return query;
    }
}