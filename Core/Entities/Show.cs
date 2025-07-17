namespace Core.Entities;

public class Show : BaseEntity
{
    public DateTime CurtainsUp { get; set; }
    public required string Day {  set; get; }
}

