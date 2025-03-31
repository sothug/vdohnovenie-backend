using System.Security.Claims;
using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/galleryitems")]
[ApiController]
[Authorize(Policy = "Authenticated")]
public class GalleryItemsController : GenericController<GalleryItem, GalleryItemFilter>
{
    public GalleryItemsController(IRepository<GalleryItem> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    [AllowAnonymous]
    public override async Task<IActionResult> GetAll([FromQuery] GalleryItemFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.GalleryItems.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<GalleryItem>(items, PageNumber, PageSize, totalItems));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Create([FromBody] GalleryItem entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        entity.AuthorId = userId;
        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] GalleryItem entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (entity.AuthorId != userId) return Forbid();
        return await base.Update(id, entity);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Delete(int id)
    {
        var item = await _context.GalleryItems.FindAsync(id);
        if (item == null) return NotFound();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (item.AuthorId != userId) return Forbid();
        return await base.Delete(id);
    }
}