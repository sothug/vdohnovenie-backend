using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class ReviewFilter : IFilter<Review>
{
    public int? AuthorId { get; set; }
    public int? CoachId { get; set; }
    public int? ScheduleId { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }

    public IQueryable<Review> Apply(IQueryable<Review> query, ClaimsPrincipal user)
    {
        if (AuthorId.HasValue) query = query.Where(r => r.AuthorId == AuthorId);
        if (CoachId.HasValue) query = query.Where(r => r.CoachId == CoachId);
        if (ScheduleId.HasValue) query = query.Where(r => r.ScheduleId == ScheduleId);
        if (MinRating.HasValue) query = query.Where(r => r.Rating >= MinRating);
        if (MaxRating.HasValue) query = query.Where(r => r.Rating <= MaxRating);

        return query;
    }
}