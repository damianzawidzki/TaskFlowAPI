using System.Data;

namespace TaskFlowAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CreatedBy { get; set; } = DateTime.UtcNow;
        public List<TaskItem> Tasks { get; set; } = new();
    }
}
