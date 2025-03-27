using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class GymHallFilter : IFilter<GymHall>
{
    public string? NameContains { get; set; }

    public IQueryable<GymHall> Apply(IQueryable<GymHall> query, ClaimsPrincipal user)
    {
        if (!string.IsNullOrEmpty(NameContains)) query = query.Where(g => g.Name.Contains(NameContains));
        return query;
    }
}