using System.Data;
using ClosedXML.Attributes;
using ClosedXML.Excel;
using MediatR;

namespace API.Commands.ExportReservations;

public class ExportReservationCommandHandler : IRequestHandler<ExportReservationCommand, Byte[]>
{
    public async Task<byte[]> Handle(ExportReservationCommand request, CancellationToken cancellationToken)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("Email",  typeof(string));
        dataTable.Columns.Add("Voornaam",  typeof(string));
        dataTable.Columns.Add("Naam",  typeof(string));
        dataTable.Columns.Add("Show",  typeof(string));
        dataTable.Columns.Add("Aantal volwassenen",  typeof(string));
        dataTable.Columns.Add("Aantal kinderen",  typeof(string));

        List<string> headers = ["Email", "Voornaam", "Naam", "Show", "Aantal volwassenen", "Aantal kinderen"];
        dataTable.Rows.Add(headers.ToArray());

        var reservations = request.ReservationDtos;

        foreach ( var reservation in reservations )
        {
            List<string> reservationLine = 
            [
                reservation.Email,
                reservation.Name,
                reservation.SurName,
                reservation.ShowId == 1 ? "Zaterdag" : "Zondag",
                reservation.NumberOfAdults.ToString(),
                reservation.NumberOfChildren.ToString()
            ];
            dataTable.Rows.Add( reservationLine.ToArray() );
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reservations");
        worksheet.Cell("A1").InsertData(dataTable);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return stream.ToArray();
    }
}

public record ReservationLine(
    [property:XLColumn(Order = 1)] string Naam,
    [property:XLColumn(Order = 2)] string Voornaam,
    [property:XLColumn(Order = 3)] string Show,
    [property:XLColumn(Order = 4)] int AantalVolwassenen,
    [property:XLColumn(Order = 5)] int AantalKinderen
    );
