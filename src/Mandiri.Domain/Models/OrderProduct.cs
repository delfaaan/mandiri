namespace Mandiri.Domain.Entities
{
	public class OrderProduct
	{
		public int OrderId { get; set; }
		public Order Order { get; set; } = null!;
		public int ProductId { get; set; }
		public Product Product { get; set; } = null!;
		public int Quantity { get; set; } = 1;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	}
}