namespace Mandiri.Domain.Entities
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
	}
}