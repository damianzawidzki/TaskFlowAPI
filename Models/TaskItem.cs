using System.ComponentModel;

namespace TaskFlowAPI.Models
{
    public class TaskItem
    {
        public int TaskItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        
        //Foreign Key
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        //Foreign Key
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
