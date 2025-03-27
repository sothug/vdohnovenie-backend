using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class UserFilter : IFilter<User>
{
    public int? ParentId { get; set; }
    public string Role { get; set; }
    public string? FullNameContains { get; set; }

    public IQueryable<User> Apply(IQueryable<User> query, ClaimsPrincipal user)
    {
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (userRole == "Parent")
            query = query.Where(u => u.Id == userId || u.ParentId == userId || u.Role == "Coach");
        else if (userRole == "Child")
            query = query.Where(u => u.Id == userId || u.Id == u.ParentId || u.Role == "Coach");

        if (ParentId.HasValue) query = query.Where(u => u.ParentId == ParentId);
        if (!string.IsNullOrEmpty(Role)) query = query.Where(u => u.Role == Role);
        if (!string.IsNullOrEmpty(FullNameContains)) query = query.Where(u => u.FullName.Contains(FullNameContains));

        return query;
    }
}