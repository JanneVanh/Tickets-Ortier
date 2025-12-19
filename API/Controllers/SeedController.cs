using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class SeedController : ControllerBase
{
    private readonly TicketContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<SeedController> _logger;

    public SeedController(
        TicketContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<SeedController> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpPost("migrate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> MigrateDatabase()
    {
        try
        {
            _logger.LogInformation("Starting database migrations at {Time}", DateTime.UtcNow);
            
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var pendingMigrationsList = pendingMigrations.ToList();
            
            if (!pendingMigrationsList.Any())
            {
                _logger.LogInformation("No pending migrations found");
                return Ok(new 
                { 
                    message = "No pending migrations. Database is up to date.",
                    timestamp = DateTime.UtcNow
                });
            }
            
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migrations completed at {Time}", DateTime.UtcNow);
            
            return Ok(new 
            { 
                message = "Database migrations completed successfully",
                appliedMigrations = pendingMigrationsList,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database migration");
            return StatusCode(500, new 
            { 
                message = "Error migrating database",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> SeedDatabase()
    {
        try
        {
            _logger.LogInformation("Starting database seeding at {Time}", DateTime.UtcNow);

            // Run migrations first
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migrations completed");

            // Seed the database
            await TicketContextSeed.SeedAsync(_context, _userManager, _roleManager);
            _logger.LogInformation("Database seeding completed at {Time}", DateTime.UtcNow);

            return Ok(new
            {
                message = "Database seeded successfully",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            return StatusCode(500, new
            {
                message = "Error seeding database",
                error = ex.Message
            });
        }
    }
}
