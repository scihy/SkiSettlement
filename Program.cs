using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SkiSettlement.Components;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;
using SkiSettlement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();

// Entity Framework Core z SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (logowanie e-mail + hasło)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<SkiSettlement.Data.ApplicationUserClaimsPrincipalFactory>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

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
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// MudBlazor
builder.Services.AddMudServices();

var app = builder.Build();

// Zastosowanie migracji przy starcie (tworzy lub aktualizuje tabele)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    // Upewnij się, że brakujące kolumny są dodane (bez logowania błędów gdy już istnieją)
    EnsureColumn(db, "InstructorWeeks", "SupplementName", "ALTER TABLE InstructorWeeks ADD COLUMN SupplementName TEXT;");
    EnsureColumn(db, "InstructorWeeks", "SupplementAmount", "ALTER TABLE InstructorWeeks ADD COLUMN SupplementAmount TEXT NOT NULL DEFAULT '0';");
    EnsureColumn(db, "Expenses", "IsPaid", "ALTER TABLE Expenses ADD COLUMN IsPaid INTEGER NOT NULL DEFAULT 0;");
    EnsureColumn(db, "Incomes", "IsPaid", "ALTER TABLE Incomes ADD COLUMN IsPaid INTEGER NOT NULL DEFAULT 0;");
    EnsureAuditLogTable(db);
    EnsureIdentityTables(db);
}

static void EnsureColumn(AppDbContext db, string table, string columnName, string alterSql)
{
    var conn = db.Database.GetDbConnection();
    if (conn.State != System.Data.ConnectionState.Open) conn.Open();
    using var cmd = conn.CreateCommand();
    // table i columnName są stałymi z kodu, nie wejściem użytkownika
    cmd.CommandText = $"SELECT 1 FROM pragma_table_info('{table.Replace("'", "''")}') WHERE name='{columnName.Replace("'", "''")}' LIMIT 1;";
    var exists = cmd.ExecuteScalar() != null;
    if (!exists)
        db.Database.ExecuteSqlRaw(alterSql);
}

static void EnsureAuditLogTable(AppDbContext db)
{
    var conn = db.Database.GetDbConnection();
    if (conn.State != System.Data.ConnectionState.Open) conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS AuditLogs (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            CreatedAt TEXT NOT NULL,
            UserName TEXT,
            Action TEXT NOT NULL,
            EntityType TEXT NOT NULL,
            EntityId INTEGER,
            Description TEXT NOT NULL
        );";
    cmd.ExecuteNonQuery();
}

static void EnsureIdentityTables(AppDbContext db)
{
    var conn = db.Database.GetDbConnection();
    if (conn.State != System.Data.ConnectionState.Open) conn.Open();
    var sql = new[]
    {
        @"CREATE TABLE IF NOT EXISTS AspNetRoles (
            Id TEXT NOT NULL PRIMARY KEY,
            Name TEXT,
            NormalizedName TEXT,
            ConcurrencyStamp TEXT
        );",
        @"CREATE TABLE IF NOT EXISTS AspNetUsers (
            Id TEXT NOT NULL PRIMARY KEY,
            UserName TEXT,
            NormalizedUserName TEXT,
            Email TEXT,
            NormalizedEmail TEXT,
            EmailConfirmed INTEGER NOT NULL,
            PasswordHash TEXT,
            SecurityStamp TEXT,
            ConcurrencyStamp TEXT,
            PhoneNumber TEXT,
            PhoneNumberConfirmed INTEGER NOT NULL,
            TwoFactorEnabled INTEGER NOT NULL,
            LockoutEnd TEXT,
            LockoutEnabled INTEGER NOT NULL,
            AccessFailedCount INTEGER NOT NULL,
            FirstName TEXT NOT NULL DEFAULT '',
            LastName TEXT NOT NULL DEFAULT ''
        );",
        @"CREATE TABLE IF NOT EXISTS AspNetUserRoles (
            UserId TEXT NOT NULL,
            RoleId TEXT NOT NULL,
            PRIMARY KEY (UserId, RoleId),
            FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
            FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
        );",
        @"CREATE TABLE IF NOT EXISTS AspNetUserClaims (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            UserId TEXT NOT NULL,
            ClaimType TEXT,
            ClaimValue TEXT,
            FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
        );",
        @"CREATE TABLE IF NOT EXISTS AspNetUserLogins (
            LoginProvider TEXT NOT NULL,
            ProviderKey TEXT NOT NULL,
            ProviderDisplayName TEXT,
            UserId TEXT NOT NULL,
            PRIMARY KEY (LoginProvider, ProviderKey),
            FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
        );",
        @"CREATE TABLE IF NOT EXISTS AspNetUserTokens (
            UserId TEXT NOT NULL,
            LoginProvider TEXT NOT NULL,
            Name TEXT NOT NULL,
            Value TEXT,
            PRIMARY KEY (UserId, LoginProvider, Name),
            FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
        );",
        @"CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            RoleId TEXT NOT NULL,
            ClaimType TEXT,
            ClaimValue TEXT,
            FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
        );"
    };
    foreach (var s in sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = s;
        cmd.ExecuteNonQuery();
    }
    // Kolumny FirstName, LastName mogły nie istnieć w istniejącej tabeli AspNetUsers (stworzonej wcześniej)
    EnsureColumn(db, "AspNetUsers", "FirstName", "ALTER TABLE AspNetUsers ADD COLUMN FirstName TEXT NOT NULL DEFAULT '';");
    EnsureColumn(db, "AspNetUsers", "LastName", "ALTER TABLE AspNetUsers ADD COLUMN LastName TEXT NOT NULL DEFAULT '';");
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
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
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
