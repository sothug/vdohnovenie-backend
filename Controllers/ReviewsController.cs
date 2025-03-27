using System.Security.Claims;
using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/reviews")]
[ApiController]
public class ReviewsController : GenericController<Review, ReviewFilter>
{
    public ReviewsController(IRepository<Review> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    [AllowAnonymous] // Доступно всем
    public override async Task<IActionResult> GetAll([FromQuery] ReviewFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.Reviews.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var reviews = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<Review>(reviews, PageNumber, PageSize, totalItems));
    }

    [HttpPost]
    [Authorize(Policy = "Authenticated")] // Только авторизованные пользователи могут оставлять отзывы
    public override async Task<IActionResult> Create([FromBody] Review entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        entity.AuthorId = userId;
        entity.CreatedAt = DateTime.UtcNow;

        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Authenticated")] // Только автор может редактировать свой отзыв
    public override async Task<IActionResult> Update(int id, [FromBody] Review entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var existingReview = await _context.Reviews.FindAsync(id);
        if (existingReview == null) return NotFound();
        if (existingReview.AuthorId != userId) return Forbid();

        entity.AuthorId = userId; // Защита от изменения авторства
        entity.CreatedAt = existingReview.CreatedAt; // Сохраняем оригинальную дату
        return await base.Update(id, entity);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Authenticated")] // Только автор или админ может удалять
    public override async Task<IActionResult> Delete(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (review.AuthorId != userId && userRole != "Admin") return Forbid();

        return await base.Delete(id);
    }
}