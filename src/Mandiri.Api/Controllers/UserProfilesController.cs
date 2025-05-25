using Mandiri.Domain.Entities;
using Mandiri.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Mandiri.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserProfilesController : ControllerBase
	{
		private readonly IRepository<UserProfile> _repository;
		private readonly IRepository<User> _userRepository;
		private readonly IMapper _mapper;

		public UserProfilesController(IRepository<UserProfile> repository, IRepository<User> userRepository, IMapper mapper)
		{
			_repository = repository;
			_userRepository = userRepository;
			_mapper = mapper;
		}

		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetByUserId(int userId)
		{
			var profile = (await _repository.GetAllAsync()).FirstOrDefault(p => p.UserId == userId);

			return profile == null ? NotFound() : Ok(_mapper.Map<UserProfileResponse>(profile));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UserProfileRequest request)
		{
			var user = await _userRepository.GetByIdAsync(request.UserId);

			if (user == null) return BadRequest("User not found");

			var profile = _mapper.Map<UserProfile>(request);

			await _repository.AddAsync(profile);
			await _repository.SaveAsync();

			return CreatedAtAction(nameof(GetByUserId), new { userId = request.UserId }, _mapper.Map<UserProfileResponse>(profile));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UserProfileRequest request)
		{
			var existingProfile = await _repository.GetByIdAsync(id);

			if (existingProfile == null) return NotFound();

			_mapper.Map(request, existingProfile);

			await _repository.UpdateAsync(existingProfile);
			await _repository.SaveAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var profile = await _repository.GetByIdAsync(id);

			if (profile == null) return NotFound();

			await _repository.DeleteAsync(profile);
			await _repository.SaveAsync();
			
			return NoContent();
		}
	}

	public class UserProfileRequest
	{
		public string Address { get; set; } = string.Empty;
		public int UserId { get; set; }
	}

	public class UserProfileResponse
	{
		public int Id { get; set; }
		public string Address { get; set; } = string.Empty;
		public int UserId { get; set; }
	}
}