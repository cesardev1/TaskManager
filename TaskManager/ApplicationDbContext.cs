using Microsoft.EntityFrameworkCore;
using TaskManager.Entities;

namespace TaskManager;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options): base(options) 
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<Step> Steps { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }
}