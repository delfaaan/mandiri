using Mandiri.Domain.Entities;
using Mandiri.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Mandiri.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderProductsController : ControllerBase
	{
		private readonly IRepository<OrderProduct> _repository;
		private readonly IRepository<Order> _orderRepository;
		private readonly IRepository<Product> _productRepository;
		private readonly IMapper _mapper;

		public OrderProductsController(IRepository<OrderProduct> repository, IRepository<Order> orderRepository, IRepository<Product> productRepository, IMapper mapper)
		{
			_repository = repository;
			_orderRepository = orderRepository;
			_productRepository = productRepository;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var orderProducts = await _repository.GetAllAsync();

			return Ok(_mapper.Map<IEnumerable<OrderProductResponse>>(orderProducts));
		}

		[HttpGet("{orderId}/{productId}")]
		public async Task<IActionResult> GetById(int orderId, int productId)
		{
			var orderProduct = (await _repository.GetAllAsync()).FirstOrDefault(op => op.OrderId == orderId && op.ProductId == productId);

			if (orderProduct == null) return NotFound();

			return Ok(_mapper.Map<OrderProductResponse>(orderProduct));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] OrderProductRequest request)
		{
			var order = await _orderRepository.GetByIdAsync(request.OrderId);

			if (order == null) return BadRequest(new { Message = "Order not found" });

			var product = await _productRepository.GetByIdAsync(request.ProductId);

			if (product == null) return BadRequest(new { Message = "Product not found" });

			var orderProduct = _mapper.Map<OrderProduct>(request);

			await _repository.AddAsync(orderProduct);
			await _repository.SaveAsync();

			return CreatedAtAction(nameof(GetById), new { orderId = orderProduct.OrderId, productId = orderProduct.ProductId }, _mapper.Map<OrderProductResponse>(orderProduct));
		}

		[HttpPut("{orderId}/{productId}")]
		public async Task<IActionResult> Update(int orderId, int productId, [FromBody] OrderProductUpdateRequest request)
		{
			try
			{
				await _repository.BeginTransactionAsync();

				var existingOP = (await _repository.GetAllAsync()).FirstOrDefault(op => op.OrderId == orderId && op.ProductId == productId);

				if (existingOP == null)
				{
					await _repository.RollbackTransactionAsync();

					return NotFound();
				}

				if (request.NewProductId.HasValue && request.NewProductId != productId)
				{
					var newProduct = await _productRepository.GetByIdAsync(request.NewProductId.Value);

					if (newProduct == null)
					{
						await _repository.RollbackTransactionAsync();

						return BadRequest("New product not found");
					}

					await _repository.DeleteAsync(existingOP);

					var newOP = new OrderProduct
					{
						OrderId = orderId,
						ProductId = request.NewProductId.Value,
						Quantity = request.Quantity
					};

					await _repository.AddAsync(newOP);
				}
				else
				{
					existingOP.Quantity = request.Quantity;

					await _repository.UpdateAsync(existingOP);
				}

				await _repository.SaveAsync();
				await _repository.CommitTransactionAsync();

				return NoContent();
			}
			catch
			{
				await _repository.RollbackTransactionAsync();

				throw;
			}
		}

		[HttpDelete("{orderId}/{productId}")]
		public async Task<IActionResult> Delete(int orderId, int productId)
		{
			var orderProduct = (await _repository.GetAllAsync()).FirstOrDefault(op => op.OrderId == orderId && op.ProductId == productId);

			if (orderProduct == null) return NotFound();

			await _repository.DeleteAsync(orderProduct);
			await _repository.SaveAsync();
			
			return NoContent();
		}
	}

	public class OrderProductRequest
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; } = 1;
	}

	public class OrderProductUpdateRequest
	{
		public int? NewProductId { get; set; }
		public int Quantity { get; set; }
	}

	public class OrderProductResponse
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}