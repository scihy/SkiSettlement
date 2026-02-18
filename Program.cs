using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SkiSettlement.Components;
using SkiSettlement.Data;
using SkiSettlement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Entity Framework Core z SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Serwisy biznesowe
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITripExcelExportService, TripExcelExportService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<IInstructorExcelExportService, InstructorExcelExportService>();
builder.Services.AddScoped<IInstructorWeekService, InstructorWeekService>();

// MudBlazor
builder.Services.AddMudServices();

var app = builder.Build();

// Zastosowanie migracji przy starcie (tworzy lub aktualizuje tabele)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/trips/{id:int}/export", async (int id, ITripService tripService, IExpenseService expenseService, IIncomeService incomeService, ITripExcelExportService excelExport) =>
{
    var trip = await tripService.GetByIdAsync(id);
    if (trip is null)
        return Results.NotFound();
    var expenses = await expenseService.GetByTripIdAsync(id);
    var incomes = await incomeService.GetByTripIdAsync(id);
    var bytes = excelExport.ExportTripDetails(trip, expenses, incomes);
    var fileName = $"wyjazd-{id}-{trip.Name.Replace("\"", "").Replace(":", "-")}.xlsx";
    if (fileName.Length > 100) fileName = $"wyjazd-{id}.xlsx";
    return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
});

app.MapGet("/api/instructors/export", async (IInstructorService instructorService, IInstructorExcelExportService excelExport) =>
{
    var list = await instructorService.GetAllAsync();
    var bytes = excelExport.ExportAll(list);
    return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rozliczenia-instruktorow.xlsx");
});

app.MapGet("/api/instructors/{id:int}/export", async (int id, IInstructorService instructorService, IInstructorExcelExportService excelExport) =>
{
    var instructor = await instructorService.GetByIdAsync(id);
    if (instructor is null)
        return Results.NotFound();
    var bytes = excelExport.ExportOne(instructor);
    var fileName = $"instruktor-{id}-{instructor.LastName}_{instructor.FirstName}.xlsx".Replace(" ", "-");
    if (fileName.Length > 80) fileName = $"instruktor-{id}.xlsx";
    return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
});

app.Run();
