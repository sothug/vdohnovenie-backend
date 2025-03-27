using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/subscriptiontypes")]
public class SubscriptionTypesController : GenericController<SubscriptionType, SubscriptionTypeFilter>
{
    public SubscriptionTypesController(IRepository<SubscriptionType> repository, SportClubContext context) : base(repository, context) { }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Create([FromBody] SubscriptionType entity)
    {
        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] SubscriptionType entity)
    {
        return await base.Update(id, entity);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Delete(int id)
    {
        return await base.Delete(id);
    }
}