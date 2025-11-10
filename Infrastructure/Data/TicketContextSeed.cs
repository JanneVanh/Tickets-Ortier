using System.Text.Json;
using API.Enums;
using Core.Entities;

namespace Infrastructure.Data;

public class TicketContextSeed
{
    public static async Task SeedAsync(TicketContext context)
    {
        var showData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/Shows.json");
        var shows = JsonSerializer.Deserialize<List<Show>>(showData);

        if (!context.Shows.Any())
        {
            if (shows is null) return;

            context.Shows.AddRange(shows);
            await context.SaveChangesAsync();
        }

        var seatData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/Seats.json");
        var seats = JsonSerializer.Deserialize<List<Seat>>(seatData);

        if (!context.Seats.Any())
        {
            if (seats is null) return;

            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();
        }

        if (!context.SeatHolds.Any())
        {
            if (seats is null) return;
            if (shows is null) return;

            var seatHolds = new List<SeatHold>();
            var seatsNamesToHold = new List<string> { "A4", "A5", "A6", "A7", "A8", "A9", "A10", "A11",
                "A12", "A13", "A14", "A15", "B8", "B9", "B10", "B11", "C9", "C10", "C11", "D9", "D10",
                "T8", "T9", "T10", "T11", "T12", "T13", "U7", "U8", "U9", "U10", "U11", "U12", "U13"};

            foreach (var show in shows)
            {
                var seatHoldsToAdd = seats.Where(s => seatsNamesToHold.Contains(s.Name)).ToList()
                    .Select(s => new SeatHold
                    {
                        SeatId = s.Id,
                        ShowId = show.Id,
                        Status = SeatStatus.Reserved,
                        HoldStartTime = DateTime.Now,
                        HoldExpiryTime = DateTime.MaxValue,
                    });

                seatHolds.AddRange(seatHoldsToAdd);
            }

            if (seatHolds is null) return;

            context.SeatHolds.AddRange(seatHolds);
            await context.SaveChangesAsync();
        }

    }

}

