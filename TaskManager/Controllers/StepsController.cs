using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Entities;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers;

[Route("api/steps")]
public class StepsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUserServices _userServices;

    public StepsController(ApplicationDbContext context,
                            IUserServices userServices)
    {
        _context = context;
        _userServices = userServices;
    }

    [HttpPost("{todoId:int}")]
    public async Task<ActionResult<Step>> Post(int todoId, [FromBody] StepCreateDTO stepCreateDto)
    {
        var userId = _userServices.GetUserById();
        var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoId);

        if (todo is null)
            return NotFound();
        
        if(todo.CreatedByUserId != userId)
            return Forbid();

        var existSteps = await _context.Steps.AnyAsync(p=> p.TodoItemId == todoId);
        
        var orderMax = 0;

        if (existSteps)
        {
            orderMax = await _context.Steps.Where(p=> p.TodoItemId == todoId).Select(p=>p.OrderIndex).MaxAsync();
        }

        var step = new Step();
        step.TodoItemId = todoId;
        step.OrderIndex = orderMax + 1;
        step.Description = stepCreateDto.Description;
        step.IsCompleted = stepCreateDto.IsCompleted;
        
        _context.Add(step);
        await _context.SaveChangesAsync();
        return step;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] StepCreateDTO stepCreateDto)
    {
        var userId = _userServices.GetUserById();

        var step = await _context.Steps
                                .Include(s => s.TodoItem)
                                .FirstOrDefaultAsync(s => s.Id == id);
        
        if (step is null)
            return NotFound();
        
        if(step.TodoItem.CreatedByUserId != userId)
            return Forbid();
        
        step.Description = stepCreateDto.Description;
        step.IsCompleted = stepCreateDto.IsCompleted;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = _userServices.GetUserById();
        var step = await _context.Steps.Include(p=>p.TodoItem)
            .FirstOrDefaultAsync(p=>p.Id == id);;
        
        if(step is null)
            return NotFound();
        
        if(step.TodoItem.CreatedByUserId != userId)
            return Forbid();
        
        _context.Remove(step);
        await _context.SaveChangesAsync();
        return Ok();
    }
}