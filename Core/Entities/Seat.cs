namespace Core.Entities;

public class Seat : BaseEntity
{
    public required char Row { get; init; }
    public int Number {  get; init; }
    public bool IsWheelchair { get; init; } = false;
    public string Name => string.Concat(Row, Number.ToString());
}

