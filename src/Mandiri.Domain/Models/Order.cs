namespace Mandiri.Domain.Entities
{
	public class Order
	{
		public int Id { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public int UserId { get; set; }
		public User User { get; set; } = null!;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
	}
}