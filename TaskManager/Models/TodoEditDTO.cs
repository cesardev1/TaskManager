using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TodoEditDTO
{
    [Required]
    [StringLength(250)]
    public string Title { get; set; }
    public string Description { get; set; }
}