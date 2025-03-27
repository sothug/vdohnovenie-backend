using System.Security.Claims;
using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/news")]
[ApiController]
// [Authorize(Policy = "Authenticated")]
public class NewsController : GenericController<News, NewsFilter>
{
    public NewsController(IRepository<News> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    [AllowAnonymous]
    public override async Task<IActionResult> GetAll([FromQuery] NewsFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.News.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var news = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<News>(news, PageNumber, PageSize, totalItems));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Create([FromBody] News entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        entity.AuthorId = userId;
        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] News entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (entity.AuthorId != userId) return Forbid();
        return await base.Update(id, entity);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Delete(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null) return NotFound();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (news.AuthorId != userId) return Forbid();
        return await base.Delete(id);
    }
}