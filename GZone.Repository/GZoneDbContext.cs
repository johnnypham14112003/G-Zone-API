using GZone.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.X86;

namespace GZone.Repository
{
    public class GZoneDbContext : DbContext
    {
        //Constructor
        public GZoneDbContext() { }
        public GZoneDbContext(DbContextOptions<GZoneDbContext> options) : base(options) { }

        //Binding Models
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customization> Customizations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderVoucher> OrderVouchers { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<RatingComment> RatingComments { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserVoucher> UserVouchers { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<WarrantyClaim> WarrantyClaims { get; set; }

        // --- Config Connection (for reading from appsettings.json) ---
        //private static string GetConnectionString()
        //{
        //    string root = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
        //    string apiDirectory = Path.Combine(root, "GZone.API");
        //    IConfiguration configuration = new ConfigurationBuilder()
        //        .SetBasePath(apiDirectory)
        //        .AddJsonFile("appsettings.json", true, true).Build();
        //    return configuration["ConnectionStrings:DefaultConnection"]!;
        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(GetConnectionString());
        //    }
        //}

        //Config Model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Account
            modelBuilder.Entity<Account>(acc =>
            {
                acc.HasMany(a => a.CartItems)
                .WithOne(cart => cart.Account)
                .HasForeignKey(cart => cart.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete CartItems

                acc.HasMany(acc => acc.Customizations)
                .WithOne(cus => cus.Customer)
                .HasForeignKey(cus => cus.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Customizations

                acc.HasMany(acc => acc.ConfirmedCustomizations)
               .WithOne(u => u.Staff)
               .HasForeignKey(u => u.ConfirmedByStaffId)
               .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete ConfirmedCustomizations

                acc.HasMany(acc => acc.Orders)
                .WithOne(ord => ord.Customer)
                .HasForeignKey(ord => ord.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Orders

                acc.HasMany(acc => acc.ManagedOrders)
                .WithOne(ord => ord.Staff)
                .HasForeignKey(ord => ord.ManagedByStaffId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete ManagedOrders

                acc.HasMany(acc => acc.Transactions)
                .WithOne(t => t.Staff)
                .HasForeignKey(t => t.VerifiedByStaffId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Transactions

                acc.HasMany(acc => acc.RatingComments)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete RatingComments

                acc.HasMany(acc => acc.UserAddresses)
                .WithOne(ua => ua.Account)
                .HasForeignKey(ua => ua.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete UserAddresses

                acc.HasMany(acc => acc.UserNotifications)
                .WithOne(un => un.Account)
                .HasForeignKey(un => un.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

                acc.HasMany(acc => acc.UserVouchers)
                .WithOne(uv => uv.Account)
                .HasForeignKey(uv => uv.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

                acc.HasMany(acc => acc.CreatedVouchers)
                .WithOne(v => v.Staff)
                .HasForeignKey(v => v.CreatedByStaffId)
                .OnDelete(DeleteBehavior.Cascade);

                acc.HasMany(acc => acc.CreatedClaims)
                .WithOne(w => w.Customer)
                .HasForeignKey(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

                acc.HasMany(acc => acc.ProcessedClaims)
                .WithOne(w => w.Staff)
                .HasForeignKey(w => w.ProcessedByStaffId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            //CartItem
            modelBuilder.Entity<CartItem>(cart =>
            {
                cart.HasOne(c => c.Account)
                .WithMany(acc => acc.CartItems)
                .HasForeignKey(c => c.AccountId);

                cart.HasOne(c => c.ProductVariant)
                .WithOne(pv => pv.CartItem)
                .HasForeignKey<CartItem>(c => c.ProductVariantId);
            });

            //Category
            modelBuilder.Entity<Category>(cate =>
            {
                cate.HasOne(c => c.ParentCategory)
                .WithMany(ca => ca.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId);

                cate.HasMany(c => c.Products)
                .WithOne(pro => pro.Category)
                .HasForeignKey(pro => pro.CategoryId);
            });

            //Customization
            modelBuilder.Entity<Customization>(cus =>
            {
                cus.HasOne(c => c.Customer)
                .WithMany(acc => acc.Customizations)
                .HasForeignKey(c => c.CustomerId);

                cus.HasOne(c => c.Staff)
                .WithMany(acc => acc.ConfirmedCustomizations)
                .HasForeignKey(c => c.ConfirmedByStaffId);

                cus.HasOne(c => c.Product)
                .WithMany(pro => pro.Customizations)
                .HasForeignKey(c => c.ProductId);

                cus.HasMany(c => c.Images)
                .WithOne(im => im.Customization)
                .HasForeignKey(im => im.CustomId);

                cus.HasOne(c => c.OrderDetail)
                .WithOne(ordde => ordde.Customization)
                .HasForeignKey<OrderDetail>(ordde => ordde.CustomizationId);
            });

            //Image
            modelBuilder.Entity<Image>(ima =>
            {
                ima.HasOne(i => i.Product)
                .WithMany(pro => pro.Images)
                .HasForeignKey(i => i.ProductId);

                ima.HasOne(i => i.Customization)
                .WithMany(cus => cus.Images)
                .HasForeignKey(i => i.CustomId);

                ima.HasOne(i => i.ProductVariant)
                .WithMany(pv => pv.Images)
                .HasForeignKey(i => i.ProductVariantId);

                ima.HasOne(i => i.Rating)
                .WithMany(rate => rate.RatingImages)
                .HasForeignKey(i => i.RatingId);
            });

            //Notification
            modelBuilder.Entity<Notification>(noti =>
            {
                noti.HasMany(n => n.UserNotifications)
                .WithOne(un => un.Notification)
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);      // when delete Notification - delete UserNotification

                noti.HasOne(n => n.Order)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => n.ReferenceId);

                noti.HasOne(n => n.Customization)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.ReferenceId);

                noti.HasOne(n => n.WarrantyClaim)
                .WithMany(wa => wa.Notifications)
                .HasForeignKey(n => n.ReferenceId);
            });

            //Order
            modelBuilder.Entity<Order>(ord =>
            {
                ord.HasMany(o => o.OrderDetails)
                .WithOne(ordde => ordde.Order)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);      // when delete Order - delete OrderDetails

                ord.HasOne(o => o.Customer)
                .WithMany(acc => acc.Orders)
                .HasForeignKey(o => o.CustomerId);

                ord.HasOne(o => o.Staff)
                .WithMany(acc => acc.ManagedOrders)
                .HasForeignKey(o => o.ManagedByStaffId);

                ord.HasMany(o => o.OrderVouchers)
                .WithOne(ov => ov.Order)
                .HasForeignKey(o => o.OrderId);

                ord.HasMany(o => o.Transactions)
                .WithOne(t => t.Order)
                .HasForeignKey(o => o.OrderId);

                ord.HasMany(o => o.Notifications)
                .WithOne(n => n.Order)
                .HasForeignKey(o => o.NotificationId);
            });

            //OrderDetail
            modelBuilder.Entity<OrderDetail>(orde =>
            {
                orde.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

                orde.HasOne(od => od.Customization)
                .WithOne(cus => cus.OrderDetail)
                .HasForeignKey<OrderDetail>(od => od.CustomizationId);

                orde.HasOne(od => od.ProductVariant)
                .WithOne(prov => prov.OrderDetails)
                .HasForeignKey<OrderDetail>(od => od.ProductVariantId);

                orde.HasMany(od => od.WarrantyClaims)
                .WithOne(wa => wa.OrderDetail)
                .HasForeignKey(od => od.OrderDetailId);
            });

            //OrderVoucher
            modelBuilder.Entity<OrderVoucher>(ordv =>
            {
                ordv.HasKey(ov => new { ov.OrderId, ov.VoucherId });

                ordv.HasOne(ov => ov.Order)
                .WithMany(o => o.OrderVouchers)
                .HasForeignKey(ov => ov.OrderId);

                ordv.HasOne(ov => ov.Voucher)
                .WithMany(v => v.OrderVouchers)
                .HasForeignKey(ov => ov.VoucherId);
            });

            //PaymentTransaction
            modelBuilder.Entity<PaymentTransaction>(trans =>
            {
                trans.HasOne(pt => pt.Order)
                .WithMany(o => o.Transactions)
                .HasForeignKey(pt => pt.OrderId);

                trans.HasOne(pt => pt.Staff)
                .WithMany(acc => acc.Transactions)
                .HasForeignKey(pt => pt.VerifiedByStaffId);
            });

            //Product
            modelBuilder.Entity<Product>(prod =>
            {
                prod.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(c => c.CategoryId);

                prod.HasMany(p => p.Customizations)
                .WithOne(cus => cus.Product)
                .HasForeignKey(cus => cus.ProductId);

                prod.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

                prod.HasMany(p => p.ProductVariants)
                .WithOne(pvar => pvar.Product)
                .HasForeignKey(pvar => pvar.ProductId);

                prod.HasMany(p => p.RatingComments)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId);
            });

            //ProductVariant
            modelBuilder.Entity<ProductVariant>(prodvar =>
            {
                prodvar.HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId);

                prodvar.HasOne(pv => pv.CartItem)
                .WithOne(ci => ci.ProductVariant)
                .HasForeignKey<CartItem>(ci => ci.ProductVariantId);

                prodvar.HasOne(pv => pv.OrderDetails)
                .WithOne(od => od.ProductVariant)
                .HasForeignKey<OrderDetail>(od => od.ProductVariantId);

                prodvar.HasMany(pv => pv.Images)
                .WithOne(i => i.ProductVariant)
                .HasForeignKey(i => i.ProductVariantId);
            });

            //RatingComment
            modelBuilder.Entity<RatingComment>(rate =>
            {
                rate.HasOne(r => r.Product)
                .WithMany(p => p.RatingComments)
                .HasForeignKey(r => r.ProductId);

                rate.HasOne(r => r.Customer)
                .WithMany(acc => acc.RatingComments)
                .HasForeignKey(r => r.CustomerId);

                rate.HasOne(r => r.OrderDetail)
                .WithMany(od => od.RatingComments)
                .HasForeignKey(r => r.OrderDetailId);

                rate.HasMany(r => r.RatingImages)
                .WithOne(i => i.Rating)
                .HasForeignKey(i => i.RatingId);
            });

            //UserAddress
            modelBuilder.Entity<UserAddress>(usad =>
            {
                usad.HasOne(ua => ua.Account)
                .WithMany(acc => acc.UserAddresses)
                .HasForeignKey(ua => ua.AccountId);
            });

            // UserNotification
            modelBuilder.Entity<UserNotification>(usno =>
            {
                usno.HasKey(un => new { un.AccountId, un.NotificationId });

                usno.HasOne(un => un.Account)
                .WithMany(acc => acc.UserNotifications)
                .HasForeignKey(un => un.AccountId);

                usno.HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId);
            });

            // UserVoucher
            modelBuilder.Entity<UserVoucher>(usad =>
            {
                usad.HasKey(ua => new { ua.AccountId, ua.VoucherId });

                usad.HasOne(ua => ua.Account)
                .WithMany(acc => acc.UserVouchers)
                .HasForeignKey(ua => ua.AccountId);

                usad.HasOne(ua => ua.Voucher)
                .WithMany(v => v.UserVouchers)
                .HasForeignKey(ua => ua.VoucherId);
            });


            // voucher
            modelBuilder.Entity<Voucher>(vou =>
            {
                vou.HasOne(v => v.Staff)
                .WithMany(acc => acc.CreatedVouchers)
                .HasForeignKey(v => v.CreatedByStaffId);

                vou.HasMany(v => v.OrderVouchers)
                .WithOne(ov => ov.Voucher)
                .HasForeignKey(ov => ov.VoucherId);
            });


            // WarrantyClaim
            modelBuilder.Entity<WarrantyClaim>(wc =>
            {
                wc.HasOne(w => w.Customer)
                .WithMany(acc => acc.CreatedClaims)
                .HasForeignKey(w => w.CustomerId);

                wc.HasOne(w => w.Staff)
                .WithMany(acc => acc.ProcessedClaims)
                .HasForeignKey(w => w.ProcessedByStaffId);

                wc.HasOne(w => w.OrderDetail)
                .WithMany(od => od.WarrantyClaims)
                .HasForeignKey(w => w.OrderDetailId);

                wc.HasMany(w => w.Notifications)
                .WithOne(n => n.WarrantyClaim)
                .HasForeignKey(n => n.NotificationId);
            });
        }
    }
}
