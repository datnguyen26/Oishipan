using Microsoft.EntityFrameworkCore;

namespace Oishipan.Models
{
    public class OishipanContext : DbContext
    {
        public OishipanContext(DbContextOptions<OishipanContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình Unique cho Email và Phone trong bảng Account
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email).IsUnique();
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.PhoneNumber).IsUnique();

            // Cấu hình Role enum lưu dưới dạng chuỗi và map tới cột Role
            modelBuilder.Entity<Account>()
                .Property(a => a.UserRole)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnName("Role")
                .IsRequired();

            // Cấu hình Unique cho Voucher Code
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code).IsUnique();
        }
    }
}