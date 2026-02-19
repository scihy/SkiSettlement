using ClosedXML.Excel;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class InstructorExcelExportService : IInstructorExcelExportService
{
    public byte[] ExportOne(Instructor i)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Rozliczenie", 1);
        int row = 1;

        sheet.Cell(row, 1).Value = $"{i.FirstName} {i.LastName}";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row += 2;

        sheet.Cell(row, 1).Value = "Pensja podstawowa:";
        sheet.Cell(row, 2).Value = (double)i.BaseSalary;
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
        row++;
        sheet.Cell(row, 1).Value = "Suma godzin:";
        sheet.Cell(row, 2).Value = (double)i.TotalHours;
        row++;
        sheet.Cell(row, 1).Value = "Stawka za godzinę:";
        sheet.Cell(row, 2).Value = (double)i.HourlyRate;
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
        row++;
        sheet.Cell(row, 1).Value = "Kwota za godziny:";
        sheet.Cell(row, 2).Value = (double)i.AmountFromHours;
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
        row++;
        if (i.TotalSupplementAmount != 0)
        {
            sheet.Cell(row, 1).Value = "Dodatki łącznie:";
            sheet.Cell(row, 2).Value = (double)i.TotalSupplementAmount;
            sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }
        sheet.Cell(row, 1).Value = "Razem:";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = (double)i.TotalAmount;
        sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
        row += 2;

        if (i.Weeks.Any())
        {
            sheet.Cell(row, 1).Value = "Tygodnie pracy";
            sheet.Cell(row, 1).Style.Font.Bold = true;
            row++;
            sheet.Cell(row, 1).Value = "Tydzień";
            sheet.Cell(row, 2).Value = "Godziny";
            sheet.Cell(row, 3).Value = "Kwota (godz. × stawka)";
            sheet.Cell(row, 4).Value = "Dodatek (nazwa)";
            sheet.Cell(row, 5).Value = "Kwota dodatku";
            sheet.Range(row, 1, row, 5).Style.Font.Bold = true;
            row++;

            foreach (var w in i.Weeks.OrderBy(x => x.WeekNumber))
            {
                sheet.Cell(row, 1).Value = $"Tydzień {w.WeekNumber}";
                sheet.Cell(row, 2).Value = (double)w.HoursWorked;
                sheet.Cell(row, 3).Value = (double)(w.HoursWorked * i.HourlyRate);
                sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                sheet.Cell(row, 4).Value = w.SupplementName ?? "";
                sheet.Cell(row, 5).Value = (double)w.SupplementAmount;
                sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                row++;
            }
        }

        sheet.Column(1).AdjustToContents();
        sheet.Column(2).AdjustToContents();
        sheet.Column(3).AdjustToContents();
        sheet.Column(4).AdjustToContents();
        sheet.Column(5).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream, false);
        return stream.ToArray();
    }

    public byte[] ExportAll(IReadOnlyList<Instructor> instructors)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Instruktorzy", 1);
        int row = 1;

        sheet.Cell(row, 1).Value = "Imię";
        sheet.Cell(row, 2).Value = "Nazwisko";
        sheet.Cell(row, 3).Value = "Pensja podstawowa";
        sheet.Cell(row, 4).Value = "Godziny";
        sheet.Cell(row, 5).Value = "Kwota (godz.)";
        sheet.Cell(row, 6).Value = "Dodatek (nazwa)";
        sheet.Cell(row, 7).Value = "Kwota dodatku";
        sheet.Cell(row, 8).Value = "Razem";
        sheet.Range(row, 1, row, 8).Style.Font.Bold = true;
        row++;

        foreach (var i in instructors)
        {
            sheet.Cell(row, 1).Value = i.FirstName;
            sheet.Cell(row, 2).Value = i.LastName;
            sheet.Cell(row, 3).Value = (double)i.BaseSalary;
            sheet.Cell(row, 4).Value = (double)i.TotalHours;
            sheet.Cell(row, 5).Value = (double)i.AmountFromHours;
            sheet.Cell(row, 6).Value = i.TotalSupplementAmount != 0 ? "dodatki z tygodni" : "";
            sheet.Cell(row, 7).Value = (double)i.TotalSupplementAmount;
            sheet.Cell(row, 8).Value = (double)i.TotalAmount;
            sheet.Range(row, 3, row, 5).Style.NumberFormat.Format = "#,##0.00";
            sheet.Range(row, 7, row, 8).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }

        sheet.Column(1).AdjustToContents();
        sheet.Column(2).AdjustToContents();
        sheet.Column(3).AdjustToContents();
        sheet.Column(4).AdjustToContents();
        sheet.Column(5).AdjustToContents();
        sheet.Column(6).AdjustToContents();
        sheet.Column(7).AdjustToContents();
        sheet.Column(8).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream, false);
        return stream.ToArray();
    }
}
