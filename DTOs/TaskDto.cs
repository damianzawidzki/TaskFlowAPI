using System.Reflection;

namespace TaskFlowAPI.DTOs
{
    public class TaskDto
    {
        public int TaskItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Priority {  get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string CategoryName {  get; set; } = string.Empty;
        public string UserName {  get; set; } = string.Empty;
    }
}
