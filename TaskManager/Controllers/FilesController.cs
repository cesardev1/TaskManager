using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Entities;
using TaskManager.Services;

namespace TaskManager.Controllers;

[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStore _fileStore;
    private readonly IUserServices _userServices;
    private readonly string ContainerName = "attachments";

    public FilesController(ApplicationDbContext context,
                            IFileStore fileStore,
                            IUserServices userServices)
    {
        _context = context;
        _fileStore = fileStore;
        _userServices = userServices;
    }

    [HttpPost("{todoId:int}")]
    public async Task<ActionResult<IEnumerable<FileAttachment>>> Post(int todoId,
        [FromForm] IEnumerable<IFormFile> files)
    {
        var userId = _userServices.GetUserById();
        var todo = await _context.TodoItems.FirstOrDefaultAsync(t=> t.Id == todoId);
        
        if(todo is null)
            return NotFound();
        
        if(todo.CreatedByUserId != userId)
            return Forbid();
        
        var existAttachments = await _context.FileAttachments.AnyAsync(a=>a.TodoItemId == todoId);
        
        var orderMax = 0;

        if (existAttachments)
        {
            orderMax = await _context.FileAttachments.Where(a=>a.TodoItemId == todoId).Select(a=>a.OrderIndex).MaxAsync();
        }

        var result = await _fileStore.Store(ContainerName, files);
        
        var attachments = result.Select((result,index)=> new FileAttachment
        {
            TodoItemId = todoId,
            CreatedAt = DateTime.UtcNow,
            Url = result.URL,
            Title = result.Title,
            OrderIndex = orderMax + index + 1
        }).ToList();
        
        _context.AddRange(attachments);
        await _context.SaveChangesAsync();

        return attachments.ToList();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] string title)
    {
        var userId = _userServices.GetUserById();
        var fileAttached = await _context.FileAttachments
                                    .Include(a => a.TodoItem)
                                    .FirstOrDefaultAsync(a => a.Id == id);
        
        if(fileAttached is null)
            return NotFound();
        
        if(fileAttached.TodoItem.CreatedByUserId != userId)
            return Forbid();
        
        fileAttached.Title = title;
        await _context.SaveChangesAsync();
        return Ok();
    }
}