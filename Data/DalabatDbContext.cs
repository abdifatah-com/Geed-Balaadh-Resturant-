using Microsoft.EntityFrameworkCore;
using dalabat.Models;

namespace dalabat.Data;

public class DalabatDbContext : DbContext
{
    public DalabatDbContext(DbContextOptions<DalabatDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderProgress> OrderProgresses => Set<OrderProgress>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map Table Names to database tables
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Restaurant>().ToTable("Restaurants");
        modelBuilder.Entity<FoodItem>().ToTable("Food_Items");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderItem>().ToTable("Order_Items");
        modelBuilder.Entity<OrderProgress>().ToTable("Order_Progress");
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<Review>().ToTable("Reviews");
        modelBuilder.Entity<UserAddress>().ToTable("User_Addresses");
        modelBuilder.Entity<Coupon>().ToTable("Coupons");
        modelBuilder.Entity<Driver>().ToTable("Drivers");
        modelBuilder.Entity<Payment>().ToTable("Payments");
        modelBuilder.Entity<Reservation>().ToTable("Reservations");

        // Primary Keys configuration
        modelBuilder.Entity<User>().HasKey(u => u.UserID);
        modelBuilder.Entity<Restaurant>().HasKey(r => r.RestaurantID);
        modelBuilder.Entity<FoodItem>().HasKey(f => f.FoodID);
        modelBuilder.Entity<Order>().HasKey(o => o.OrderID);
        modelBuilder.Entity<OrderItem>().HasKey(oi => oi.OrderItemID);
        modelBuilder.Entity<OrderProgress>().HasKey(op => op.ProgressID);
        modelBuilder.Entity<Category>().HasKey(c => c.CategoryID);
        modelBuilder.Entity<Review>().HasKey(rv => rv.ReviewID);
        modelBuilder.Entity<UserAddress>().HasKey(ua => ua.AddressID);
        modelBuilder.Entity<Coupon>().HasKey(cp => cp.CouponID);
        modelBuilder.Entity<Driver>().HasKey(d => d.DriverID);
        modelBuilder.Entity<Payment>().HasKey(p => p.PaymentID);
        modelBuilder.Entity<Reservation>().HasKey(res => res.ReservationID);

        // Decimal Precision
        modelBuilder.Entity<FoodItem>().Property(f => f.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderItem>().Property(oi => oi.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Coupon>().Property(c => c.DiscountPercentage).HasColumnType("decimal(5,2)");
        modelBuilder.Entity<Coupon>().Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");

        // Relationships

        // FoodItem <-> Restaurant
        modelBuilder.Entity<FoodItem>()
            .HasOne(f => f.Restaurant)
            .WithMany(r => r.FoodItems)
            .HasForeignKey(f => f.RestaurantID)
            .OnDelete(DeleteBehavior.Cascade);

        // FoodItem <-> Category
        modelBuilder.Entity<FoodItem>()
            .HasOne(f => f.Category)
            .WithMany(c => c.FoodItems)
            .HasForeignKey(f => f.CategoryID)
            .OnDelete(DeleteBehavior.SetNull);

        // Order <-> User
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Order <-> Restaurant
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Restaurant)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => o.RestaurantID)
            .OnDelete(DeleteBehavior.Restrict);

        // Order <-> Driver
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Driver)
            .WithMany(d => d.Orders)
            .HasForeignKey(o => o.DriverID)
            .OnDelete(DeleteBehavior.SetNull);

        // OrderItem <-> Order
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItem <-> FoodItem
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.FoodItem)
            .WithMany(f => f.OrderItems)
            .HasForeignKey(oi => oi.FoodID)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderProgress <-> Order
        modelBuilder.Entity<OrderProgress>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProgresses)
            .HasForeignKey(op => op.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        // Review <-> User & Restaurant
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Restaurant)
            .WithMany(rest => rest.Reviews)
            .HasForeignKey(r => r.RestaurantID)
            .OnDelete(DeleteBehavior.Cascade);

        // UserAddress <-> User
        modelBuilder.Entity<UserAddress>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(ua => ua.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment <-> Order
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        // Reservation <-> User & Restaurant
        modelBuilder.Entity<Reservation>()
            .HasOne(res => res.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(res => res.UserID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Reservation>()
            .HasOne(res => res.Restaurant)
            .WithMany(rest => rest.Reservations)
            .HasForeignKey(res => res.RestaurantID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
