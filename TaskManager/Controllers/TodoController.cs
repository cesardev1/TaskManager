using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Entities;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers;

[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;

    public TodoController(ApplicationDbContext context,
                            IUserServices userServices,
                            IMapper mapper)
    {
        _context = context;
        _userServices = userServices;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoDTO>>> Get()
    {
        var userId = _userServices.GetUserById();
        var todos = await _context.TodoItems
                    .Where(t=> t.CreatedByUserId == userId)
                    .OrderBy(t=> t.OrderIndex)
                    .ProjectTo<TodoDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();

        return todos;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Post([FromBody] string title)
    {
        var userId = _userServices.GetUserById();
        var hasTasks = await _context.TodoItems.AnyAsync(t => t.CreatedByUserId == userId);

        var orderMax = 0;

        if (hasTasks)
            orderMax = await _context.TodoItems.Where(t => t.CreatedByUserId == userId).Select(t => t.OrderIndex)
                .MaxAsync();

        var todo = new TodoItem
        {
            Title = title,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            OrderIndex = orderMax + 1
        };

        _context.Add(todo);
        await _context.SaveChangesAsync();

        return todo;
    }
    
    
}