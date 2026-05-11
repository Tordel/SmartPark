using Microsoft.EntityFrameworkCore;
using SmartPark.Domain.Entities;

namespace SmartPark.Infrastructure.Persistence;

public class SmartParkDbContext : DbContext
{
    public SmartParkDbContext(DbContextOptions<SmartParkDbContext> options) : base(options) { }
    
    public DbSet<ParkingSpace> ParkingSpaces { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ParkingSpace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SpaceNumber).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Type).HasConversion<string>();
        });

        modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.OwnsOne(e => e.Interval, interval =>
                {
                    interval.Property(i => i.Start).HasColumnName("IntervalStart");
                    interval.Property(i => i.End).HasColumnName("IntervalEnd");
                });
                entity.HasOne<ParkingSpace>()
                    .WithMany()
                    .HasForeignKey(e => e.ParkingSpaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

                entity.HasData(new
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Username = "admin",
                    PasswordHash = "$2a$11$P.jbOn7PTxNqBnHcTGSyk.mUoE1gdQ4bg5hVKBF/nT27xKhY1RclK",
                    Role = "Admin"
                });
            });
    }
}