using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<InstructorWeek> InstructorWeeks => Set<InstructorWeek>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Destination).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(e => e.Trip).WithMany(t => t.Expenses).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(e => e.Trip).WithMany(t => t.Incomes).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<DictionaryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DefaultAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BaseSalary).HasPrecision(18, 2);
            entity.Property(e => e.HourlyRate).HasPrecision(18, 2);
        });
        modelBuilder.Entity<InstructorWeek>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HoursWorked).HasPrecision(18, 2);
            entity.HasOne(e => e.Instructor).WithMany(i => i.Weeks).HasForeignKey(e => e.InstructorId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
