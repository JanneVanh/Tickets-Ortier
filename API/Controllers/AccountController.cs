using API.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[Controller]")]
public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        // Assign default "User" role to new registrations
        await userManager.AddToRoleAsync(user, "User");

        return Ok(new { Message = "Registration successful" });
    }

    [HttpGet("auth-status")]
    public async Task<ActionResult> GetAuthState()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                return Ok(new 
                { 
                    IsAuthenticated = true,
                    Email = user.Email,
                    Roles = roles
                });
            }
        }
        
        return Ok(new { IsAuthenticated = false });
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                return Ok(new 
                { 
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles
                });
            }
        }
        return Unauthorized();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }
}