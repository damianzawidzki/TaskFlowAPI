using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Models;
namespace TaskFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get api/Task
        //Return all tasks beloging to the currently logged-in user.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("Invalid or missing user token.");
            }

            var tasks = await _context.Tasks
                .Where(task => task.UserId == userId.Value)
                .Include(task => task.Category)
                .Include(task => task.User)
                .OrderBy(task => task.IsCompleted)
                .ThenBy(task => task.DueDate)
                .Select(task => new TaskDto
                {
                    TaskItemId = task.TaskItemId,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    IsCompleted = task.IsCompleted,
                    CategoryName = task.Category.Name,
                    UserName = task.User.FirstName + " " + task.User.LastName,
                })
                .ToListAsync();

            return Ok(tasks);
        }

        //Get api/task/5
        //Returns one task belonging to the currently logged-in user.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null) 
            {
                return Unauthorized("Invalid or missing user token.");
            }

            var task = await _context.Tasks
                .Where(task =>
                    task.TaskItemId == id &&
                    task.UserId == userId.Value)
                .Include(task => task.Category)
                .Include(task => task.User)
                .Select(task => new TaskDto
                {
                    TaskItemId = task.TaskItemId,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    IsCompleted = task.IsCompleted,
                    CategoryName = task.Category.Name,
                    UserName = task.User.FirstName + "" + task.User.LastName,
                })
                .FirstOrDefaultAsync();
            if (task == null)
            {
                return NotFound("Task was not found.");
            }
            return Ok(task);

        }

        //POST: api/Task
        //Creates a new task for the currently logged-in user.
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("Invalid or missing user token.");
            }

            var categoryExists = await _context.Categories
                .AnyAsync(category => category.CategoryId == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest("Selected category does not exist.");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate.ToUniversalTime(),
                Priority = dto.Priority,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                CategoryId = dto.CategoryId,
                UserId = userId.Value
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var createdTask = await _context.Tasks
                .Include(item => item.Category)
                .Include(item => item.User)
                .Where(item => item.TaskItemId == task.TaskItemId)
                .Select(item => new TaskDto
                {
                    TaskItemId = item.TaskItemId,
                    Title = item.Title,
                    Description = item.Description,
                    DueDate = item.DueDate,
                    Priority = item.Priority,
                    IsCompleted = item.IsCompleted,
                    CategoryName = item.Category.Name,
                    UserName = item.User.FirstName + " " + item.User.LastName,
                })
                .FirstAsync();

            return CreatedAtAction(
                nameof(GetTask),
                new { id = task.TaskItemId },
                createdTask);
        }

        //PUT: api/Task/5
        //Updates an existing task belonging to the logged-in user.
        [HttpPut("{id:int")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("Invalid or missing user token.");
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(item =>
                    item.TaskItemId == id &&
                    item.UserId == userId.Value);

            if (task == null)
            {
                return NotFound("Task was not found.");
            }

            var categoryExists = await _context.Categories
                .AnyAsync(category => category.CategoryId == dto.CategoryId);

            if (categoryExists)
            {
                return BadRequest("Selected category does not exist.");
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate.ToUniversalTime();
            task.Priority = dto.Priority;
            task.IsCompleted = dto.IsCompleted;
            task.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task updated successfully.",
                taskId = task.TaskItemId
            });
        }
        //PATCH: api/Task/5/complete
        //Mark an existing task as completed.
        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("Invalid or missing user token.");
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(item =>
                item.TaskItemId == id &&
                item.UserId == userId.Value);

            if (task == null)
            {
                return NotFound("Task was not found.");
            }

            task.IsCompleted = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task marked as complted.",
                taskId = task.TaskItemId
            });
        }

        //DELETE: api/Task/5
        //Deletes a task belonging to the logged-in user.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask (int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("Invalid or missing user token.");
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync( item =>
                item.TaskItemId == id &&
                item.UserId == userId.Value);

            if (task == null)
            {
                return NotFound("Task was not found");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task deleted successfully.",
                taskId = task.TaskItemId
            });
        }

        //Reads the logged-in user's ID from the JWT token.
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
