using Mandiri.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Mandiri.Infrastructure.Interfaces;

namespace Mandiri.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IRepository<User> _repository;
		private readonly IMapper _mapper;

		public UsersController(IRepository<User> repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var users = await _repository.GetAllAsync();

			return Ok(_mapper.Map<IEnumerable<UserResponse>>(users));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var user = await _repository.GetByIdAsync(id);

			if (user == null) return NotFound();

			return Ok(_mapper.Map<UserResponse>(user));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UserRequest request)
		{
			var user = _mapper.Map<User>(request);

			await _repository.AddAsync(user);
			await _repository.SaveAsync();

			return CreatedAtAction(nameof(GetById), new { id = user.Id }, _mapper.Map<UserResponse>(user));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UserRequest request)
		{
			var existingUser = await _repository.GetByIdAsync(id);

			if (existingUser == null) return NotFound();

			_mapper.Map(request, existingUser);

			await _repository.UpdateAsync(existingUser);
			await _repository.SaveAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var user = await _repository.GetByIdAsync(id);

			if (user == null) return NotFound();

			await _repository.DeleteAsync(user);
			await _repository.SaveAsync();

			return NoContent();
		}
	}

	public class UserRequest
	{
		public string Name { get; set; } = string.Empty;
	}

	public class UserResponse
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}