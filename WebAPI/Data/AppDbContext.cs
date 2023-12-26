using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserOperationClaim>().HasData(
                    new UserOperationClaim
                    {
                        RecordId = 1,
                        OperationClaimId = 1,
                        UserId = 1
                    }
                );
            modelBuilder.Entity<OperationClaim>().HasData(
                    new OperationClaim
                    {
                        RecordId = 1,
                        Name = "Admin"
                    },
                    new OperationClaim
                    {
                        RecordId = 2,
                        Name = "Müşteri"
                    }
                );
            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        RecordId = 1,
                        Email = "deniz@gmail.com",
                        Password = "123456",
                        Username = "denizduman"
                    },
                    new User
                    {
                        RecordId = 2,
                        Email = "alican@gmail.com",
                        Password = "1234567",
                        Username = "alicanbutun"
                    },
                    new User
                    {
                        RecordId = 3,
                        Email = "ufuk@gmail.com",
                        Password = "12345678",
                        Username = "ufukkurekci"
                    },
                    new User
                    {
                        RecordId = 4,
                        Email = "ismail@gmail.com",
                        Password = "1234",
                        Username = "ismailatli"
                    }
                );
        }
    }
}
