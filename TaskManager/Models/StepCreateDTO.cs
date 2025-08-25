using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class StepCreateDTO
{
    [Required]
    public  string Description { get; set; }
    public bool IsCompleted { get; set; }
}