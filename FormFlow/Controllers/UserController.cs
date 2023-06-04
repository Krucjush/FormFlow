using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using FormFlow.Models.Enums;

namespace FormFlow.Controllers
{
	[Controller]
	[Route("api/[controller]")]
	public class UserController : Controller
	{
		private readonly UserRepository _userRepository;
		private readonly FormRepository _formRepository;

		public UserController(UserRepository userRepository, FormRepository formRepository)
		{
			_userRepository = userRepository;
			_formRepository = formRepository;
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
			var existingUser = await _userRepository.GetByIdAsync(id);
			if (existingUser == null)
			{
				return NotFound();
			}

			// Disassociate the forms from the user by setting the OwnerId to null
			await _formRepository.DisassociateFormsFromUserAsync(id);

			await _userRepository.DeleteAsync(id);

			return NoContent();
		}
	}
}
