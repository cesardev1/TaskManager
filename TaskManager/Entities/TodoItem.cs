using System.ComponentModel.DataAnnotations;

namespace TaskManager.Entities;

public class TodoItem
{
    public int Id { get; set; }
    [StringLength(250)]
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<Step> Steps { get; set; }
    public List<FileAttachment> FileAttachments { get; set; }
}