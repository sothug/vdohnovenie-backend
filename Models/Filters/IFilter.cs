using System.Security.Claims;

namespace backend.Models.Filters;

public interface IFilter<T> where T : class, IEntity
{
    IQueryable<T> Apply(IQueryable<T> query, ClaimsPrincipal user);
}