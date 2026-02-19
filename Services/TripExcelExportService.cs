using ClosedXML.Excel;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class TripExcelExportService : ITripExcelExportService
{
    private static readonly XLColor DelicateRed = XLColor.FromArgb(255, 235, 238);   // delikatne czerwone tło
    private static readonly XLColor DelicateGreen = XLColor.FromArgb(232, 245, 233); // delikatne zielone tło

    public byte[] ExportTripDetails(Trip trip, IReadOnlyList<Expense> expenses, IReadOnlyList<Income> incomes)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Zestawienie", 1);

        int row = 1;

        // Nagłówek wyjazdu – wytłuszczone, delikatne czerwone tło
        sheet.Cell(row, 1).Value = "Wyjazd";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 1).Style.Fill.BackgroundColor = DelicateRed;
        row++;
        sheet.Cell(row, 1).Value = trip.Name;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 1).Style.Fill.BackgroundColor = DelicateRed;
        sheet.Cell(row, 2).Style.Fill.BackgroundColor = DelicateRed;
        row++;
        if (!string.IsNullOrEmpty(trip.Destination))
        {
            sheet.Cell(row, 1).Value = "Cel:";
            sheet.Cell(row, 2).Value = trip.Destination;
            sheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = DelicateRed;
            row++;
        }
        sheet.Cell(row, 1).Value = "Okres:";
        sheet.Cell(row, 2).Value = $"{trip.DateFrom:yyyy-MM-dd} – {trip.DateTo:yyyy-MM-dd}";
        sheet.Range(row, 1, row, 2).Style.Font.Bold = true;
        sheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = DelicateRed;
        row += 2;

        // Koszty wg kategorii
        var expensesByCategory = expenses
            .GroupBy(e => e.CategoryId)
            .OrderBy(g => g.Key == null ? 1 : 0)
            .ThenBy(g => g.First().Category?.Name ?? "—")
            .Select(g => (
                CategoryName: g.Key == null ? "Bez kategorii" : (g.First().Category?.Name ?? "?"),
                Items: g.ToList()
            ))
            .ToList();

        sheet.Cell(row, 1).Value = "Koszty";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row++;

        foreach (var group in expensesByCategory)
        {
            sheet.Cell(row, 1).Value = group.CategoryName;
            sheet.Cell(row, 1).Style.Font.Bold = true;
            sheet.Cell(row, 1).Style.Fill.BackgroundColor = DelicateGreen;
            sheet.Cell(row, 2).Style.Fill.BackgroundColor = DelicateGreen;
            row++;
            sheet.Cell(row, 1).Value = "Opis";
            sheet.Cell(row, 2).Value = "Kwota";
            sheet.Range(row, 1, row, 2).Style.Font.Bold = true;
            row++;
            foreach (var exp in group.Items)
            {
                sheet.Cell(row, 1).Value = exp.Description;
                sheet.Cell(row, 2).Value = (double)exp.Amount;
                sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                row++;
            }
            var sum = group.Items.Sum(e => e.Amount);
            sheet.Cell(row, 1).Value = "Suma:";
            sheet.Cell(row, 1).Style.Font.Bold = true;
            sheet.Cell(row, 2).Value = (double)sum;
            sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
            row += 2; // odstęp po kategorii
        }

        sheet.Cell(row, 1).Value = "Suma kosztów:";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = (double)expenses.Sum(e => e.Amount);
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
        row += 2;

        // Przychody
        sheet.Cell(row, 1).Value = "Przychody";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row++;
        sheet.Cell(row, 1).Value = "Opis";
        sheet.Cell(row, 2).Value = "Kwota";
        sheet.Range(row, 1, row, 2).Style.Font.Bold = true;
        row++;
        foreach (var inc in incomes)
        {
            sheet.Cell(row, 1).Value = inc.Description;
            sheet.Cell(row, 2).Value = (double)inc.Amount;
            sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }
        sheet.Cell(row, 1).Value = "Suma przychodów:";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = (double)incomes.Sum(i => i.Amount);
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
        row += 2;

        // Bilans
        var balance = incomes.Sum(i => i.Amount) - expenses.Sum(e => e.Amount);
        sheet.Cell(row, 1).Value = "Bilans:";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = (double)balance;
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";

        sheet.Column(1).AdjustToContents();
        sheet.Column(2).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream, false);
        return stream.ToArray();
    }
}
