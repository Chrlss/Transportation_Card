using Microsoft.EntityFrameworkCore;
using Transportation_Card.Models;
using Microsoft.Extensions.Logging;

namespace Transportation_Card.Data
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySQL("server=localhost;database=UsersDB;user=root;password=Family231.;")
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.CardNumber)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DateOfBirth)
                    .IsRequired();

                entity.Property(e => e.Address)
                    .IsRequired();

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Email)
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.InitialLoad)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            });
        }
    }
}
