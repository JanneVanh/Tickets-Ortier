namespace API.Dtos;

public class ReservationDto
{
    public int Id { get; set; }
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
    public string? PaymentCode { get; set; }
    public bool EmailSent { get; set; } = false;
    public List<string> Seats { get; set; }
}
