using API.Services.Contracts;
using Core.Entities;
using Core.Interfaces;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace API.Services;

public class PdfService(IShowRepository showRepository, ISeatRepository seatRepository) : IPdfService
{
    private readonly IShowRepository _showRepository = showRepository;
    private readonly ISeatRepository _seatRepository = seatRepository;

    public async Task<byte[]> GenerateTicketPdfAsync(Reservation reservation, Seat seat)
    {
        using var memoryStream = new MemoryStream();
        using var pdfWriter = new PdfWriter(memoryStream);
        using var pdfDocument = new PdfDocument(pdfWriter);
        using var document = new Document(pdfDocument);

        var show = await _showRepository.GetShowByIdAsync(reservation.ShowId);

        // Set up fonts
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        // Header
        var header = new Paragraph("KOOR ORTIER")
            .SetFont(boldFont)
            .SetFontSize(24)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.DARK_GRAY)
            .SetMarginBottom(20);
        document.Add(header);

        // Ticket title
        var ticketTitle = new Paragraph("TICKET")
            .SetFont(boldFont)
            .SetFontSize(18)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(30);
        document.Add(ticketTitle);

        // Show information
        var showInfo = new Paragraph()
            .SetFont(regularFont)
            .SetFontSize(12)
            .SetTextAlignment(TextAlignment.LEFT)
            .SetMarginBottom(20);

        showInfo.Add(new Text("Show: ").SetFont(boldFont));
        showInfo.Add($"{show.Day} - {show.CurtainsUp: dd MMMM yyyy} {show.CurtainsUp: hh:mm}\n");

        document.Add(showInfo);

        // Seat information (prominent)
        var seatInfoBox = new Div()
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .SetPadding(20)
            .SetMarginBottom(20)
            .SetTextAlignment(TextAlignment.CENTER);

        var seatInfo = new Paragraph()
            .SetFont(boldFont)
            .SetFontSize(16);

        seatInfo.Add("RIJ: ");
        seatInfo.Add(new Text(seat.Row.ToString()).SetFontSize(20).SetFontColor(ColorConstants.RED));
        seatInfo.Add("  STOEL: ");
        seatInfo.Add(new Text(seat.Number.ToString()).SetFontSize(20).SetFontColor(ColorConstants.RED));

        seatInfoBox.Add(seatInfo);
        document.Add(seatInfoBox);

        // Customer information
        var customerInfo = new Paragraph()
            .SetFont(regularFont)
            .SetFontSize(11)
            .SetMarginBottom(20);

        customerInfo.Add(new Text("Naam: ").SetFont(boldFont));
        customerInfo.Add($"{reservation.Name} {reservation.SurName}\n");
        customerInfo.Add(new Text("Reservatie code: ").SetFont(boldFont));
        customerInfo.Add($"{reservation.PaymentCode}\n");

        document.Add(customerInfo);

        // Venue information
        var venueInfo = new Paragraph()
            .SetFont(regularFont)
            .SetFontSize(11)
            .SetMarginBottom(30);

        venueInfo.Add(new Text("Locatie:\n").SetFont(boldFont));
        venueInfo.Add("Cultuurcentrum Wevelgem\n");
        venueInfo.Add("Acaciastraat 1\n");
        venueInfo.Add("8560 Wevelgem\n");

        document.Add(venueInfo);

        // Footer
        var footer = new Paragraph("Bedankt voor uw komst!")
            .SetFont(boldFont)
            .SetFontSize(12)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.DARK_GRAY)
            .SetMarginTop(40);
        document.Add(footer);

        // Barcode or QR code area (placeholder)
        var barcodeArea = new Paragraph($"Ticket ID: T{reservation.Id:D4}-S{seat.Id:D3}")
            .SetFont(regularFont)
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginTop(20);
        document.Add(barcodeArea);

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerateAllTicketsPdfAsync(Reservation reservation)
    {
        var seats = await _seatRepository.GetSeatsForReservationAsync(reservation.Id);
        var pdfList = new List<byte[]>();

        foreach (var seat in seats)
        {
            var pdf = await GenerateTicketPdfAsync(reservation, seat);
            pdfList.Add(pdf);
        }

        // make one pdf of all pages
        using var outputStream = new MemoryStream();
        using (var pdfWriter = new PdfWriter(outputStream))
        using (var mergedPdf = new PdfDocument(pdfWriter))
        {
            var merger = new iText.Kernel.Utils.PdfMerger(mergedPdf);

            foreach (var pdfBytes in pdfList)
            {
                using var pdfStream = new MemoryStream(pdfBytes);
                using var pdfDoc = new PdfDocument(new PdfReader(pdfStream));
                merger.Merge(pdfDoc, 1, pdfDoc.GetNumberOfPages());
            }
            mergedPdf.Close();
        }
        return outputStream.ToArray();
    }
}
