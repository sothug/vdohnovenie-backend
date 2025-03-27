using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class NewsFilter : IFilter<News>
{
    public int? AuthorId { get; set; }

    public IQueryable<News> Apply(IQueryable<News> query, ClaimsPrincipal user)
    {
        if (AuthorId.HasValue) query = query.Where(n => n.AuthorId == AuthorId);
        return query;
    }
}