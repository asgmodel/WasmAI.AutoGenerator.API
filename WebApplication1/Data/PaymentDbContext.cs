
using AutoGenerator;
using AutoGenerator.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WasmAI.PaymentProvider.Models;

namespace WasmAI.PaymentProvider.Data
{
    public class PaymentDbContext : AutoIdentityDataContext<ApplicationUser, IdentityRole, string>, ITAutoDbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
        public DbSet<CheckoutSession> CheckoutSessions { get; set; }

        public DbSet<ProviderPayment> PaymentProviders { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<PricePlan> PricePlans { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<PaymentMethod> PaymentMethods { get; set; } = default!;
        public DbSet<Payment> Payments { get; set; } = default!;
        public DbSet<Refund> Refunds { get; set; } = default!;
        public DbSet<Invoice> Invoices { get; set; } = default!;
        public DbSet<InvoiceItem> InvoiceItems { get; set; } = default!;
        public DbSet<Subscription> Subscriptions { get; set; } = default!;
        public DbSet<SubscriptionItem> SubscriptionItems { get; set; } = default!;
        public DbSet<Discount> Discounts { get; set; } = default!;
        public DbSet<WebhookEvent> WebhookEvents { get; set; } = default!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderPayment>().HasKey(p => p.Id);

            modelBuilder.Entity<Product>().HasKey(p => p.Id);

            modelBuilder.Entity<PricePlan>().HasKey(p => p.Id);

            modelBuilder.Entity<CheckoutSession>(entity =>
            {
                entity.HasKey(cs => cs.Id);

                entity.Property(cs => cs.Id)
                      .HasMaxLength(100);

                entity.HasOne(cs => cs.Customer)
                      .WithMany(c => c.CheckoutSessions)
                      .HasForeignKey(cs => cs.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

            });
                modelBuilder.Entity<Product>()
                .HasOne(p => p.Provider)
                .WithMany(pp => pp.Products)
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PricePlan>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.PricePlans)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.Customer)
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(pm => pm.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

           

            modelBuilder.Entity<Refund>()
               .HasOne(r => r.Payment)
               .WithMany(p => p.Refunds)
               .HasForeignKey(r => r.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(i => i.Invoice)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionItem>()
                .HasOne(si => si.Subscription)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.SubscriptionId)
                 .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Discount>()
                .HasOne(d => d.Customer)
                .WithMany(c => c.Discounts)
                .HasForeignKey(d => d.CustomerId)
                .IsRequired(false)
                 .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Customer>().HasIndex(c => c.ExternalId).IsUnique();
            modelBuilder.Entity<PaymentMethod>().HasIndex(pm => pm.ExternalId); // Non-unique per customer
            modelBuilder.Entity<Payment>().HasIndex(p => p.ExternalId).IsUnique();
            modelBuilder.Entity<Refund>().HasIndex(r => r.ExternalId).IsUnique();
            modelBuilder.Entity<Invoice>().HasIndex(i => i.ExternalId).IsUnique();
            modelBuilder.Entity<Subscription>().HasIndex(s => s.ExternalId).IsUnique();
            modelBuilder.Entity<WebhookEvent>().HasIndex(we => we.EventId).IsUnique();
            modelBuilder.Entity<Discount>().HasIndex(d => d.CouponCode).IsUnique();

            modelBuilder.Entity<PaymentMethod>().HasIndex(pm => pm.CustomerId);
            modelBuilder.Entity<Payment>().HasIndex(p => p.CustomerId);
            modelBuilder.Entity<Refund>().HasIndex(r => r.PaymentId);
            modelBuilder.Entity<Invoice>().HasIndex(i => i.CustomerId);
            modelBuilder.Entity<Subscription>().HasIndex(s => s.CustomerId);
            modelBuilder.Entity<SubscriptionItem>().HasIndex(si => si.SubscriptionId);
            modelBuilder.Entity<Discount>().HasIndex(d => d.CustomerId);


            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Refund>().Property(r => r.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceItem>().Property(ii => ii.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceItem>().Property(ii => ii.Total).HasPrecision(18, 2);
            modelBuilder.Entity<PricePlan>().Property(pp => pp.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Discount>().Property(d => d.AmountOff).HasPrecision(18, 2);


            base.OnModelCreating(modelBuilder);
        }
    }
}