using System.ComponentModel.DataAnnotations;
using backend.Database;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Authenticated")]
public class GenericController<T, TFilter> : ControllerBase 
    where T : class, IEntity, IValidatableObject 
    where TFilter : class, IFilter<T>, new()
{
    protected readonly IRepository<T> _repository;
    protected readonly SportClubContext _context;

    public GenericController(IRepository<T> repository, SportClubContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll([FromQuery] TFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.Set<T>().AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<T>(items, PageNumber, PageSize, totalItems));
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] T entity)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var validationResults = entity.Validate(new ValidationContext(entity)).ToList();
        if (validationResults.Any())
        {
            foreach (var result in validationResults)
                ModelState.AddModelError(result.MemberNames.FirstOrDefault() ?? "", result.ErrorMessage);
            return BadRequest(ModelState);
        }

        var createdEntity = await _repository.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, createdEntity);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update(int id, [FromBody] T entity)
    {
        if (id != entity.Id) return BadRequest("ID не совпадает");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var validationResults = entity.Validate(new ValidationContext(entity)).ToList();
        if (validationResults.Any())
        {
            foreach (var result in validationResults)
                ModelState.AddModelError(result.MemberNames.FirstOrDefault() ?? "", result.ErrorMessage);
            return BadRequest(ModelState);
        }

        var existingEntity = await _repository.GetByIdAsync(id);
        if (existingEntity == null) return NotFound();

        await _repository.UpdateAsync(entity);
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}