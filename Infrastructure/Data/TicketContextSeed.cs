using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class TicketContextSeed
{
    public static async Task SeedAsync(TicketContext context)
    {
        if (!context.Shows.Any())
        {
            var showData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/Shows.json");
            var shows = JsonSerializer.Deserialize<List<Show>>(showData);

            if (shows is null) return;

            context.Shows.AddRange(shows);
            await context.SaveChangesAsync();
        }

        if (!context.Seats.Any())
        {
            var seatData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/Seats.json");
            var seats = JsonSerializer.Deserialize<List<Seat>>(seatData);

            if (seats is null) return;

            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();
        }

    }

}

