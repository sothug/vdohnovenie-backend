using System.Security.Claims;
using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Dto.Schedule;
using backend.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/schedules")]
[ApiController]
[Authorize(Policy = "Authenticated")]
public class SchedulesController : GenericController<Schedule, ScheduleFilter>
{
    public SchedulesController(IRepository<Schedule> repository, SportClubContext context) : base(repository, context) { }

    [HttpGet]
    public override async Task<IActionResult> GetAll([FromQuery] ScheduleFilter filter, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = _context.Schedules.AsQueryable();
        query = filter.Apply(query, User);

        var totalItems = await query.CountAsync();
        var schedules = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Ok(new PagedResponse<Schedule>(schedules, PageNumber, PageSize, totalItems));
    }

    [HttpGet("month")]
    public async Task<IActionResult> GetSchedulesByMonth([FromQuery] int year, [FromQuery] int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var schedules = await _context.Schedules
            .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
            .GroupBy(s => s.StartTime.Date)
            .Select(g => new ScheduleMonthViewDto
            {
                Date = g.Key,
                HasFreeSlots = g.Any(s => s.IsGroupLesson || s.Attendees.Count == 0)
            })
            .ToListAsync();

        return Ok(schedules);
    }

    [HttpGet("day")]
    public async Task<IActionResult> GetSchedulesByDay([FromQuery] DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var schedules = await _context.Schedules
            .Where(s => s.StartTime >= startOfDay && s.StartTime <= endOfDay)
            .Select(s => new ScheduleDayViewDto
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Description = s.Description,
                IsGroupLesson = s.IsGroupLesson,
                IndividualPrice = s.IndividualPrice,
                IsAvailable = s.IsGroupLesson || s.Attendees.Count == 0,
                CoachFullName = s.Coach.FullName,
                GymHallName = s.GymHall.Name
            })
            .ToListAsync();

        return Ok(schedules);
    }

    [HttpPost("{id}/attend")]
    [Authorize(Policy = "ParentOnly")]
    public async Task<IActionResult> AttendSchedule(int id, [FromBody] int childId)
    {
        var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);
        if (schedule == null) return NotFound();

        var child = await _context.Users.FindAsync(childId);
        if (child == null || child.Role != "Child") return BadRequest("Неверный ребенок");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (child.ParentId != userId) return Forbid();

        if (!schedule.IsGroupLesson && schedule.Attendees.Any()) return BadRequest("Индивидуальное занятие уже занято");
        if (schedule.Attendees.Any(a => a.Id == childId)) return BadRequest("Ребенок уже записан");

        schedule.Attendees.Add(child);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}/attend/{childId}")]
    [Authorize(Policy = "ParentOnly")]
    public async Task<IActionResult> CancelAttendance(int id, int childId)
    {
        var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);
        if (schedule == null) return NotFound();

        var child = schedule.Attendees.FirstOrDefault(a => a.Id == childId);
        if (child == null) return BadRequest("Ребенок не записан");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (child.ParentId != userId) return Forbid();

        schedule.Attendees.Remove(child);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = "CoachOnly")]
    public override async Task<IActionResult> Create([FromBody] Schedule entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        entity.CoachId = userId;
        return await base.Create(entity);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CoachOnly")]
    public override async Task<IActionResult> Update(int id, [FromBody] Schedule entity)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (entity.CoachId != userId) return Forbid();
        return await base.Update(id, entity);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CoachOnly")]
    public override async Task<IActionResult> Delete(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null) return NotFound();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (schedule.CoachId != userId) return Forbid();
        return await base.Delete(id);
    }
}