using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Controller]
	[Route("api/[controller]")]
	public class UserController : Controller
	{
		private readonly UserRepository _userRepository;

		public UserController(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[HttpGet]
		public async Task<List<User>> Get()
		{
			return await _userRepository.GetAsync();
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] User user)
		{
			await _userRepository.CreateAsync(user);
			return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> AddForm(string id, [FromBody] Form form)
		{
			await _userRepository.AddFormAsync(id, form);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			await _userRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
