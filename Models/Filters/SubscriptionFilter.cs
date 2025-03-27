using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class SubscriptionFilter : IFilter<Subscription>
{
    public int? UserId { get; set; }

    public IQueryable<Subscription> Apply(IQueryable<Subscription> query, ClaimsPrincipal user)
    {
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (userRole == "Parent")
            query = query.Where(s => s.User.ParentId == userId || s.UserId == userId);
        else if (userRole == "Child")
            query = query.Where(s => s.UserId == userId);

        if (UserId.HasValue) query = query.Where(s => s.UserId == UserId);

        return query;
    }
}