using Microsoft.EntityFrameworkCore;
using Mandiri.Domain.Entities;

namespace Mandiri.Infrastructure.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users => Set<User>();
		public DbSet<UserProfile> UsersProfiles => Set<UserProfile>();
		public DbSet<Order> Orders => Set<Order>();
		public DbSet<Product> Products => Set<Product>();
		public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasOne(u => u.Profile)
				.WithOne(up => up.User)
				.HasForeignKey<UserProfile>(up => up.UserId);

			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserId);

			modelBuilder.Entity<OrderProduct>(op =>
			{
				op.HasKey(x => new { x.OrderId, x.ProductId });

				op.Property(x => x.OrderId).ValueGeneratedNever();
				op.Property(x => x.ProductId).ValueGeneratedNever();
			});
		}
	}
}