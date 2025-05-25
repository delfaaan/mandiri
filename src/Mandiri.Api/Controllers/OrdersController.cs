using Mandiri.Domain.Entities;
using Mandiri.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Mandiri.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly IRepository<Order> _repository;
		private readonly IRepository<User> _userRepository;
		private readonly IMapper _mapper;

		public OrdersController(IRepository<Order> repository, IRepository<User> userRepository, IMapper mapper)
		{
			_repository = repository;
			_userRepository = userRepository;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var orders = await _repository.GetAllAsync();

			return Ok(_mapper.Map<IEnumerable<OrderResponse>>(orders));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var order = await _repository.GetByIdAsync(id);

			if (order == null) return NotFound();

			return Ok(_mapper.Map<OrderResponse>(order));
		}

		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetByUserId(int userId)
		{
			var orders = (await _repository.GetAllAsync()).Where(o => o.UserId == userId);

			return Ok(_mapper.Map<IEnumerable<OrderResponse>>(orders));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] OrderRequest request)
		{
			var user = await _userRepository.GetByIdAsync(request.UserId);

			if (user == null) return BadRequest(new { Message = "User not found" });

			var order = _mapper.Map<Order>(request);

			await _repository.AddAsync(order);
			await _repository.SaveAsync();

			return CreatedAtAction(nameof(GetById), new { id = order.Id }, _mapper.Map<OrderResponse>(order));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] OrderRequest request)
		{
			var existingOrder = await _repository.GetByIdAsync(id);

			if (existingOrder == null) return NotFound();

			var user = await _userRepository.GetByIdAsync(request.UserId);

			if (user == null) return BadRequest(new { Message = "User not found" });

			_mapper.Map(request, existingOrder);

			await _repository.UpdateAsync(existingOrder);
			await _repository.SaveAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var order = await _repository.GetByIdAsync(id);

			if (order == null) return NotFound();

			await _repository.DeleteAsync(order);
			await _repository.SaveAsync();
			
			return NoContent();
		}
	}

	public class OrderRequest
	{
		public string ProductName { get; set; } = string.Empty;
		public int UserId { get; set; }
	}

	public class OrderResponse
	{
		public int Id { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public int UserId { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}