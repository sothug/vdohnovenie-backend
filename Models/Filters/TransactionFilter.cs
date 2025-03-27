using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class TransactionFilter : IFilter<Transaction>
{
    public int? UserId { get; set; }

    public IQueryable<Transaction> Apply(IQueryable<Transaction> query, ClaimsPrincipal user)
    {
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (userRole == "Parent")
            query = query.Where(t => (t.Subscription != null && (t.Subscription.User.ParentId == userId || t.Subscription.UserId == userId)) ||
                                     (t.Schedule != null && t.Schedule.Attendees.Any(a => a.ParentId == userId || a.Id == userId)));
        else if (userRole == "Child")
            query = query.Where(t => (t.Subscription != null && t.Subscription.UserId == userId) ||
                                     (t.Schedule != null && t.Schedule.Attendees.Any(a => a.Id == userId)));
        else if (userRole == "Coach")
            query = query.Where(t => t.Schedule != null && t.Schedule.CoachId == userId);

        if (UserId.HasValue)
            query = query.Where(t => (t.Subscription != null && t.Subscription.UserId == UserId) ||
                                     (t.Schedule != null && t.Schedule.Attendees.Any(a => a.Id == UserId)));

        return query;
    }
}