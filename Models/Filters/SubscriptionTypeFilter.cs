using System.Security.Claims;
using backend.Database.Models;

namespace backend.Models.Filters;

public class SubscriptionTypeFilter : IFilter<SubscriptionType>
{
    public IQueryable<SubscriptionType> Apply(IQueryable<SubscriptionType> query, ClaimsPrincipal user) => query;
}