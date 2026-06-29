using TaskFlowAPI.Models;

namespace TaskFlowAPI.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
