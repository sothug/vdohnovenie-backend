using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace backend.Models;

[Route("api/[controller]")]
[ApiController]
public class GenericControllerOld<T> : ControllerBase where T : class, IValidatable
{
    private readonly GenericRepository<T> _repository;

    public GenericControllerOld(GenericRepository<T> repository)
    {
        _repository = repository;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetById(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<T>> Create([FromBody] T entity)
    {
        try
        {
            var createdEntity = await _repository.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = createdEntity }, createdEntity);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<T>> Update(int id, [FromBody] T entity)
    {
        try
        {
            if (await _repository.GetByIdAsync(id) == null) return NotFound();
            return Ok(await _repository.UpdateAsync(entity));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (await _repository.DeleteAsync(id)) return NoContent();
        return NotFound();
    }
}