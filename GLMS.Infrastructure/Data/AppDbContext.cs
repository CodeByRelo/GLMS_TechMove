using GLMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =========================
        // DBSets
        // =========================
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // CLIENT → CONTRACT (1:M)
            // =========================
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CONTRACT → SERVICE REQUEST (1:M)
            // =========================
            modelBuilder.Entity<Contract>()
                .HasMany(c => c.ServiceRequests)
                .WithOne(s => s.Contract)
                .HasForeignKey(s => s.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // CONTRACT CONFIGURATION
            // =========================
            modelBuilder.Entity<Contract>()
                .Property(c => c.ServiceLevel)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Contract>()
                .Property(c => c.SignedAgreementPath)
                .HasMaxLength(255);

            // =========================
            // SERVICE REQUEST CONFIG
            // =========================
            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.Description)
                .IsRequired();

            // Currency precision (needed for financial correctness)
            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.CostUSD)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.CostZAR)
                .HasPrecision(18, 2);
        }
    }
}