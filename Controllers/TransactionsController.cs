using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/transactions")]
[ApiController]
[Authorize(Policy = "Authenticated")]
public class TransactionsController : GenericController<Transaction, TransactionFilter>
{
    public TransactionsController(IRepository<Transaction> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    public override async Task<IActionResult> GetAll([FromQuery] TransactionFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.Transactions.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var transactions = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<Transaction>(transactions, PageNumber, PageSize, totalItems));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Create([FromBody] Transaction entity)
    {
        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] Transaction entity)
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