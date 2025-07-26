using Microsoft.EntityFrameworkCore;

namespace TaskManager.Entities;

public class FileAttachment
{
    public Guid Id { get; set; }
    public int TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; }
    [Unicode(false)]
    public string Url { get; set; }
    public string Title { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
}