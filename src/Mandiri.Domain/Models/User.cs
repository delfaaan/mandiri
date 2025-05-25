namespace Mandiri.Domain.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public UserProfile Profile { get; set; } = null!;
		public ICollection<Order> Orders { get; set; } = new List<Order>();
	}
}