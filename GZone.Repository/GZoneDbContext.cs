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
                acc.Property(a => a.Salary).HasColumnType("MONEY");

                acc.HasMany(a => a.CartItems)
                .WithOne(cart => cart.Account)
                .HasForeignKey(cart => cart.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete CartItems

                acc.HasMany(acc => acc.Customizations)
                .WithOne(cus => cus.Customer)
                .HasForeignKey(cus => cus.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Customizations

                acc.HasMany(acc => acc.ConfirmedCustomizations)
               .WithOne(cus => cus.Staff)
               .HasForeignKey(cus => cus.ConfirmedByStaffId)
               .OnDelete(DeleteBehavior.Restrict);

                acc.HasMany(acc => acc.Orders)
                .WithOne(ord => ord.Customer)
                .HasForeignKey(ord => ord.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Orders

                acc.HasMany(acc => acc.ManagedOrders)
                .WithOne(ord => ord.Staff)
                .HasForeignKey(ord => ord.ManagedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

                acc.HasMany(acc => acc.Transactions)
                .WithOne(t => t.Staff)
                .HasForeignKey(t => t.VerifiedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .OnDelete(DeleteBehavior.Restrict);

                acc.HasMany(acc => acc.UserVouchers)
                .WithOne(uv => uv.Account)
                .HasForeignKey(uv => uv.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete UserVouchers

                acc.HasMany(acc => acc.CreatedVouchers)
                .WithOne(v => v.Staff)
                .HasForeignKey(v => v.CreatedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

                acc.HasMany(acc => acc.CreatedClaims)
                .WithOne(w => w.Customer)
                .HasForeignKey(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete CreatedClaims

                acc.HasMany(acc => acc.ProcessedClaims)
                .WithOne(w => w.Staff)
                .HasForeignKey(w => w.ProcessedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //CartItem
            modelBuilder.Entity<CartItem>(cart =>
            {
                cart.Property(c => c.CustomPrice).HasColumnType("MONEY");

                cart.HasOne(c => c.Account)
                .WithMany(acc => acc.CartItems)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete CartItems

                cart.HasOne(c => c.ProductVariant)
                .WithOne(pv => pv.CartItem)
                .HasForeignKey<CartItem>(c => c.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade); // when delete ProductVariant - delete CartItem
            });

            //Category
            modelBuilder.Entity<Category>(cate =>
            {
                cate.HasOne(c => c.ParentCategory)
                .WithMany(ca => ca.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                cate.HasMany(c => c.Products)
                .WithOne(pro => pro.Category)
                .HasForeignKey(pro => pro.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //Customization
            modelBuilder.Entity<Customization>(cus =>
            {
                cus.Property(c => c.QuotedPrice).HasColumnType("MONEY");
                cus.Property(c => c.Weight).HasColumnType("DECIMAL(10,2)");

                cus.HasOne(c => c.Customer)
                .WithMany(acc => acc.Customizations)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Customizations

                cus.HasOne(c => c.Staff)
                .WithMany(acc => acc.ConfirmedCustomizations)
                .HasForeignKey(c => c.ConfirmedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

                cus.HasOne(c => c.Product)
                .WithMany(pro => pro.Customizations)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Product - delete Customizations

                cus.HasMany(c => c.Images)
                .WithOne(im => im.Customization)
                .HasForeignKey(im => im.CustomId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Customization - delete Images

                cus.HasOne(c => c.OrderDetail)
                .WithOne(ordde => ordde.Customization)
                .HasForeignKey<OrderDetail>(ordde => ordde.CustomizationId)
                .OnDelete(DeleteBehavior.Restrict);

                cus.HasMany(c => c.Notifications)
                .WithOne(n => n.Customization)
                .HasForeignKey(n => n.CustomizationId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //Image
            modelBuilder.Entity<Image>(ima =>
            {
                ima.HasOne(i => i.Product)
                .WithMany(pro => pro.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

                ima.HasOne(i => i.Customization)
                .WithMany(cus => cus.Images)
                .HasForeignKey(i => i.CustomId)
                .OnDelete(DeleteBehavior.Restrict);

                ima.HasOne(i => i.ProductVariant)
                .WithMany(pv => pv.Images)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

                ima.HasOne(i => i.Rating)
                .WithMany(rate => rate.RatingImages)
                .HasForeignKey(i => i.RatingId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Rating - delete RatingImages
            });

            //Notification
            modelBuilder.Entity<Notification>(noti =>
            {
                noti.HasMany(n => n.UserNotifications)
                .WithOne(un => un.Notification)
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Notification - delete UserNotification

                noti.HasOne(n => n.Order)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete Notifications

                noti.HasOne(n => n.Customization)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CustomizationId)
                .OnDelete(DeleteBehavior.Restrict);

                noti.HasOne(n => n.WarrantyClaim)
                .WithMany(wa => wa.Notifications)
                .HasForeignKey(n => n.WarrantyClaimId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //Order
            modelBuilder.Entity<Order>(ord =>
            {
                ord.Property(o => o.DiscountAmount).HasColumnType("MONEY");
                ord.Property(o => o.ShippingFee).HasColumnType("MONEY");
                ord.Property(o => o.TotalAmount).HasColumnType("MONEY");
                ord.Property(o => o.Subtotal).HasColumnType("MONEY");
                ord.Property(o => o.TaxAmount).HasColumnType("MONEY");

                ord.HasMany(o => o.OrderDetails)
                .WithOne(ordde => ordde.Order)
                .HasForeignKey(ordde => ordde.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete OrderDetails

                ord.HasOne(o => o.Customer)
                .WithMany(acc => acc.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete Orders

                ord.HasOne(o => o.Staff)
                .WithMany(acc => acc.ManagedOrders)
                .HasForeignKey(o => o.ManagedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

                ord.HasMany(o => o.OrderVouchers)
                .WithOne(ov => ov.Order)
                .HasForeignKey(ov => ov.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete OrderVouchers

                ord.HasMany(o => o.Transactions)
                .WithOne(t => t.Order)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete PaymentTransactions

                ord.HasMany(o => o.Notifications)
                .WithOne(n => n.Order)
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete Notifications
            });

            //OrderDetail
            modelBuilder.Entity<OrderDetail>(orde =>
            {
                orde.Property(od => od.UnitPrice).HasColumnType("MONEY");
                orde.Property(od => od.TotalPrice).HasColumnType("MONEY");
                orde.Property(od => od.DiscountAmount).HasColumnType("MONEY");

                orde.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete OrderDetails

                orde.HasOne(od => od.Customization)
                .WithOne(cus => cus.OrderDetail)
                .HasForeignKey<OrderDetail>(od => od.CustomizationId)
                .OnDelete(DeleteBehavior.Restrict);

                orde.HasOne(od => od.ProductVariant)
                .WithOne(prov => prov.OrderDetails)
                .HasForeignKey<OrderDetail>(od => od.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

                orde.HasMany(od => od.RatingComments)
                .WithOne(rc => rc.OrderDetail)
                .HasForeignKey(rc => rc.OrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);

                orde.HasMany(od => od.WarrantyClaims)
                .WithOne(wa => wa.OrderDetail)
                .HasForeignKey(wa => wa.OrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //OrderVoucher
            modelBuilder.Entity<OrderVoucher>(ordv =>
            {
                ordv.HasKey(ov => new { ov.OrderId, ov.VoucherId });

                ordv.Property(ov => ov.DiscountAmount).HasColumnType("MONEY");

                ordv.HasOne(ov => ov.Order)
                .WithMany(o => o.OrderVouchers)
                .HasForeignKey(ov => ov.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete OrderVouchers

                ordv.HasOne(ov => ov.Voucher)
                .WithMany(v => v.OrderVouchers)
                .HasForeignKey(ov => ov.VoucherId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //PaymentTransaction
            modelBuilder.Entity<PaymentTransaction>(trans =>
            {
                trans.Property(a => a.Amount).HasColumnType("MONEY");

                trans.HasOne(pt => pt.Order)
                .WithMany(o => o.Transactions)
                .HasForeignKey(pt => pt.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Order - delete PaymentTransactions

                trans.HasOne(pt => pt.Staff)
                .WithMany(acc => acc.Transactions)
                .HasForeignKey(pt => pt.VerifiedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //Product
            modelBuilder.Entity<Product>(prod =>
            {
                prod.Property(p => p.AverageRating).HasColumnType("DECIMAL(5,2)");
                prod.Property(p => p.Weight).HasColumnType("DECIMAL(10,2)");
                prod.Property(p => p.BasePrice).HasColumnType("MONEY");

                prod.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                prod.HasMany(p => p.Customizations)
                .WithOne(cus => cus.Product)
                .HasForeignKey(cus => cus.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Product - delete Customizations

                prod.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

                prod.HasMany(p => p.ProductVariants)
                .WithOne(pvar => pvar.Product)
                .HasForeignKey(pvar => pvar.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Product - delete ProductVariants

                prod.HasMany(p => p.RatingComments)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //ProductVariant
            modelBuilder.Entity<ProductVariant>(prodvar =>
            {
                prodvar.Property(pv => pv.AdditionalPrice).HasColumnType("MONEY");
                prodvar.Property(pv => pv.Weight).HasColumnType("DECIMAL(10,2)");

                prodvar.HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Product - delete ProductVariants

                prodvar.HasOne(pv => pv.CartItem)
                .WithOne(ci => ci.ProductVariant)
                .HasForeignKey<CartItem>(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade); // when delete ProductVariant - delete CartItem

                prodvar.HasOne(pv => pv.OrderDetails)
                .WithOne(od => od.ProductVariant)
                .HasForeignKey<OrderDetail>(od => od.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

                prodvar.HasMany(pv => pv.Images)
                .WithOne(i => i.ProductVariant)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            //RatingComment
            modelBuilder.Entity<RatingComment>(rate =>
            {
                rate.HasOne(r => r.Product)
                .WithMany(p => p.RatingComments)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

                rate.HasOne(r => r.Customer)
                .WithMany(acc => acc.RatingComments)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete RatingComments

                rate.HasOne(r => r.OrderDetail)
                .WithMany(od => od.RatingComments)
                .HasForeignKey(r => r.OrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);

                rate.HasMany(r => r.RatingImages)
                .WithOne(i => i.Rating)
                .HasForeignKey(i => i.RatingId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Rating - delete RatingImages
            });

            //UserAddress
            modelBuilder.Entity<UserAddress>(usad =>
            {
                usad.HasOne(ua => ua.Account)
                .WithMany(acc => acc.UserAddresses)
                .HasForeignKey(ua => ua.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete UserAddresses
            });

            // UserNotification
            modelBuilder.Entity<UserNotification>(usno =>
            {
                usno.HasKey(un => new { un.AccountId, un.NotificationId });

                usno.HasOne(un => un.Account)
                .WithMany(acc => acc.UserNotifications)
                .HasForeignKey(un => un.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

                usno.HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Notification - delete UserNotifications
            });

            // UserVoucher
            modelBuilder.Entity<UserVoucher>(usad =>
            {
                usad.HasKey(ua => new { ua.AccountId, ua.VoucherId });

                usad.HasOne(ua => ua.Account)
                .WithMany(acc => acc.UserVouchers)
                .HasForeignKey(ua => ua.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete UserVouchers

                usad.HasOne(ua => ua.Voucher)
                .WithMany(v => v.UserVouchers)
                .HasForeignKey(ua => ua.VoucherId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Voucher - delete UserVouchers
            });


            // voucher
            modelBuilder.Entity<Voucher>(vou =>
            {
                vou.Property(v => v.DiscountValue).HasColumnType("MONEY");
                vou.Property(v => v.MaxDiscountAmount).HasColumnType("MONEY");

                vou.HasOne(v => v.Staff)
                .WithMany(acc => acc.CreatedVouchers)
                .HasForeignKey(v => v.CreatedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

                vou.HasMany(v => v.OrderVouchers)
                .WithOne(ov => ov.Voucher)
                .HasForeignKey(ov => ov.VoucherId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            // WarrantyClaim
            modelBuilder.Entity<WarrantyClaim>(wc =>
            {
                wc.Property(w => w.RepairCost).HasColumnType("MONEY");

                wc.HasOne(w => w.Customer)
                .WithMany(acc => acc.CreatedClaims)
                .HasForeignKey(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // when delete Account - delete CreatedClaims

                wc.HasOne(w => w.Staff)
                .WithMany(acc => acc.ProcessedClaims)
                .HasForeignKey(w => w.ProcessedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

                wc.HasOne(w => w.OrderDetail)
                .WithMany(od => od.WarrantyClaims)
                .HasForeignKey(w => w.OrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);

                wc.HasMany(w => w.Notifications)
                .WithOne(n => n.WarrantyClaim)
                .HasForeignKey(n => n.WarrantyClaimId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
