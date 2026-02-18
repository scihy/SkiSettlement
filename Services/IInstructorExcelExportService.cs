using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IInstructorExcelExportService
{
    byte[] ExportOne(Instructor instructor);
    byte[] ExportAll(IReadOnlyList<Instructor> instructors);
}
