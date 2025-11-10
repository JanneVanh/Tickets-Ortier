using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
