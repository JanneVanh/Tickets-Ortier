using API.Enums;

namespace API.Dtos;

public class SeatDto
{
    public required char Row { get; init; }
    public int Number { get; init; }
    public string Name => string.Concat(Row, Number.ToString());
    public required SeatStatus Status { get; init; }
}
