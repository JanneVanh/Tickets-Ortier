namespace Core.Entities;

public class Reservation : BaseEntity
{
    public required string SurName { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime ReservationDate { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Remark { get; set; }
    public bool IsPaid { get; set; }
    public string? PaymentRemark { get; set; }
    public required int ShowId { get; set; }
    public string? PaymentCode => $"Ortier-{Id:D3}";
}

