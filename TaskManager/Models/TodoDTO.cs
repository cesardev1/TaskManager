namespace TaskManager.Models;

public class TodoDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int StepsDone { get; set; }
    public int StepsTotal { get; set; }
}