using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class GalleryItemFilter : IFilter<GalleryItem>
{
    public int? AuthorId { get; set; }

    public IQueryable<GalleryItem> Apply(IQueryable<GalleryItem> query, ClaimsPrincipal user)
    {
        if (AuthorId.HasValue) query = query.Where(g => g.AuthorId == AuthorId);
        return query;
    }
}