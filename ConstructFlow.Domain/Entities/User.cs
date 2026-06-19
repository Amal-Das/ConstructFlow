using ConstructFlow.Domain.Common;

namespace ConstructFlow.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Admin, Manager, User
    public bool IsActive { get; set; } = true;
}