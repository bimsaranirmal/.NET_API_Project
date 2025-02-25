using Microsoft.EntityFrameworkCore;
using SPC.API.Models;
using SPC.Web.Models;

namespace SPC.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; } // Ensure this is added if OrderItem is a separate entity
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Tender> Tenders{ get; set; }
        public DbSet<SupplierDrug> SupplierDrugs { get; set; }
        public DbSet<SupplierDrugOrder> SupplierDrugOrders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define precision for decimal fields to avoid truncation issues
            modelBuilder.Entity<Drug>()
                .Property(d => d.UnitPrice)
                .HasColumnType("decimal(18,4)"); // Adjust as needed

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.UnitPrice)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Pharmacy>()
                .Property(p => p.RegistrationNumber)
                .HasMaxLength(50);

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.RegistrationNumber)
                .IsUnique();

            modelBuilder.Entity<Pharmacy>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Pharmacy>()
                .HasIndex(p => p.RegistrationNumber)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<SupplierDrug>()
                .Property(d => d.UnitPrice)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<SupplierDrugOrder>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);
        }
    }
}
