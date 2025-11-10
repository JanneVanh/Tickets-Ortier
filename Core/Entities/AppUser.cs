using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property for user roles
    public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
}

