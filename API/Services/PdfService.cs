using API.Services.Contracts;
using Core.Entities;
using Core.Interfaces;
using iText.IO.Font.Constants;
using iText.IO.Image;
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

    // Brand colors from your design system
    private static readonly DeviceRgb PrimaryBlue = new(10, 24, 155);      // #0a189b
    private static readonly DeviceRgb SecondaryGold = new(232, 177, 84);   // #e8b154
    private static readonly DeviceRgb TertiaryBlue = new(2, 24, 190);      // #0218be
    private static readonly DeviceRgb White = new(255, 255, 255);          // #ffffff


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

        // Add logo if it exists
        try
        {
            var logoPath = Path.Combine("wwwroot", "images", "Logo ortier.jpg");
            if (File.Exists(logoPath))
            {
                var logoData = ImageDataFactory.Create(logoPath);
                var logo = new Image(logoData)
                    .SetWidth(80)
                    .SetHeight(60)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10);

                // Center the logo
                var logoContainer = new Div()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20);
                logoContainer.Add(logo);
                document.Add(logoContainer);
            }
        }
        catch (Exception ex)
        {
            // Logo loading failed, continue without logo
            Console.WriteLine($"Could not load logo: {ex.Message}");
        }

        // Header
        var header = new Paragraph("KOOR ORTIER")
            .SetFont(boldFont)
            .SetFontSize(24)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(PrimaryBlue)
            .SetMarginBottom(20);
        document.Add(header);

        // Ticket title
        var ticketTitle = new Paragraph("TICKET")
            .SetFont(boldFont)
            .SetFontSize(18)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(SecondaryGold)
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

        // Seat information with brand styling
        var seatInfoBox = new Div()
            .SetBackgroundColor(TertiaryBlue)
            .SetPadding(20)
            .SetMarginBottom(20)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBorder(new iText.Layout.Borders.SolidBorder(PrimaryBlue, 2));

        var seatInfo = new Paragraph()
            .SetFont(boldFont)
            .SetFontSize(16)
            .SetFontColor(White);

        seatInfo.Add("RIJ: ");
        seatInfo.Add(new Text(seat.Row.ToString()).SetFontSize(20).SetFontColor(SecondaryGold));
        seatInfo.Add("  STOEL: ");
        seatInfo.Add(new Text(seat.Number.ToString()).SetFontSize(20).SetFontColor(SecondaryGold));

        seatInfoBox.Add(seatInfo);
        document.Add(seatInfoBox);

        // Customer information with brand styling
        var customerInfo = new Paragraph()
            .SetFont(regularFont)
            .SetFontSize(11)
            .SetMarginBottom(20);

        customerInfo.Add(new Text("Naam: ").SetFont(boldFont).SetFontColor(PrimaryBlue));
        customerInfo.Add($"{reservation.Name} {reservation.SurName}\n");
        customerInfo.Add(new Text("Reservatie code: ").SetFont(boldFont).SetFontColor(PrimaryBlue));
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

        // Footer with brand styling
        var footer = new Paragraph("Bedankt voor uw komst!")
            .SetFont(boldFont)
            .SetFontSize(12)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(SecondaryGold)
            .SetMarginTop(40);
        document.Add(footer);

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
