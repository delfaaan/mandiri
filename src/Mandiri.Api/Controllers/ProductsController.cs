using Mandiri.Domain.Entities;
using Mandiri.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Mandiri.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IRepository<Product> _repository;
		private readonly IMapper _mapper;

		public ProductsController(IRepository<Product> repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _repository.GetAllAsync();

			return Ok(_mapper.Map<IEnumerable<ProductResponse>>(products));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var product = await _repository.GetByIdAsync(id);

			if (product == null) return NotFound();

			return Ok(_mapper.Map<ProductResponse>(product));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] ProductRequest request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var product = _mapper.Map<Product>(request);

			await _repository.AddAsync(product);
			await _repository.SaveAsync();

			return CreatedAtAction(nameof(GetById), new { id = product.Id }, _mapper.Map<ProductResponse>(product));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] ProductRequest request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var existingProduct = await _repository.GetByIdAsync(id);

			if (existingProduct == null) return NotFound();

			_mapper.Map(request, existingProduct);

			await _repository.UpdateAsync(existingProduct);
			await _repository.SaveAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var product = await _repository.GetByIdAsync(id);

			if (product == null) return NotFound();

			await _repository.DeleteAsync(product);
			await _repository.SaveAsync();
			
			return NoContent();
		}
	}

	public class ProductRequest
	{
		public string Name { get; set; } = string.Empty;
	}

	public class ProductResponse
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
	}
}