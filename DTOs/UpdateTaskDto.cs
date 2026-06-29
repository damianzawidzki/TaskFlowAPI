using Microsoft.VisualBasic;

namespace TaskFlowAPI.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public bool IsCompleted { get; set; }
        public int CategoryId { get; set; }
    }
}
