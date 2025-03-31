using System.Security.Claims;
using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/users")]
[ApiController]
// [Authorize(Policy = "Authenticated")]
// [Authorize()]
public class UsersController : GenericController<User, UserFilter>
{
    public UsersController(IRepository<User> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    // [Authorize(Policy = null)]
    [AllowAnonymous]
    public override async Task<IActionResult> GetAll([FromQuery] UserFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        // var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        // var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (!User.Identity.IsAuthenticated && filter.Role != "Coach") 
            return Unauthorized();
        
        var query = _context.Users.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var users = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<User>(users, PageNumber, PageSize, totalItems));
    }

    [HttpGet("me")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] User entity)
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