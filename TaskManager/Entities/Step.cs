namespace TaskManager.Entities;

public class Step
{
    public Guid Id { get; set; }
    public int TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public int OrderIndex { get; set; }
}