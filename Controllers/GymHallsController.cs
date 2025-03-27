using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/gymhalls")]
public class GymHallsController : GenericController<GymHall, GymHallFilter>
{
    public GymHallsController(IRepository<GymHall> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    public override async Task<IActionResult> GetAll([FromQuery] GymHallFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.GymHalls.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var gymHalls = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<GymHall>(gymHalls, PageNumber, PageSize, totalItems));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Create([FromBody] GymHall entity)
    {
        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] GymHall entity)
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