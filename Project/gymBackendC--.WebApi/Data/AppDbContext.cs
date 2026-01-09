using gymBackendC__.WebApi.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.WebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Users> Users => Set<Users>();
    public DbSet<ApiKeys> ApiKeys => Set<ApiKeys>();
    public DbSet<Trainings> Trainings => Set<Trainings>();
    public DbSet<Exercises> Exercises => Set<Exercises>();
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id").IsRequired(true);
            entity.Property(e => e.Username).HasColumnName("username").IsRequired(true);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired(true);
            entity.Property(e => e.Password).HasColumnName("password").IsRequired(true);
            entity.Property(e => e.Role).HasColumnName("role").IsRequired(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired(true);
        });
        
        modelBuilder.Entity<ApiKeys>(entity =>
        {
            entity.ToTable("api_keys");
            entity.Property(e => e.Id).HasColumnName("id").IsRequired(true);
            entity.Property(e => e.Key).HasColumnName("key").IsRequired(true);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Expiration).HasColumnName("expiration").IsRequired(true);
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(true);
        });

        modelBuilder.Entity<Trainings>(entity =>
        {
            entity.ToTable("trainings");
            entity.Property(e => e.Id).HasColumnName("id").IsRequired(true);
            entity.Property(e => e.Date).HasColumnName("date").IsRequired(true);
            entity.Property(e => e.Notes).HasColumnName("notes").IsRequired(false);
            entity
                .HasMany(e => e.Users)
                .WithMany(p => p.Trainings)
                .UsingEntity<Dictionary<string, object>>(
                    "users_trainings",
                    j => j.HasOne<Users>().WithMany().HasForeignKey("user_id"),
                    j => j.HasOne<Trainings>().WithMany().HasForeignKey("training_id"),
                    j =>
                    {
                        j.HasKey("user_id", "training_id");
                    }
                );
        });
        
        modelBuilder.Entity<Exercises>(entity =>
        {
            entity.ToTable("exercises");
            entity.Property(e => e.Id).HasColumnName("id").IsRequired(true);
            entity.Property(e => e.TrainingId).HasColumnName("training_id").IsRequired(true);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired(true);
            entity.Property(e => e.Repeats).HasColumnName("repeats").IsRequired(true);
            entity.Property(e => e.Weight).HasColumnName("weight").IsRequired(true);
            entity.Property(e => e.TimeSec).HasColumnName("time_sec").IsRequired(false);
            entity.HasOne(e => e.Training).WithMany(p => p.Exercises).HasForeignKey(e => e.TrainingId).IsRequired(true);
        });
    }
}