using System.IdentityModel.Tokens.Jwt;
using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Identity;
using FormFlow.JWT;

namespace FormFlow.Controllers
{
    [Controller]
	[Route("[controller]")]
	public class UserController : Controller
	{
		private readonly UserRepository _userRepository;
		private readonly FormRepository _formRepository;
		private readonly JwtSettings _jwtSettings;

		public UserController(UserRepository userRepository, FormRepository formRepository, JwtSettings jwtSettings)
		{
			_userRepository = userRepository;
			_formRepository = formRepository;
			_jwtSettings = jwtSettings;
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
		[HttpGet("register")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(UserRegistrationModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = new User
			{
				Email = model.Email,
				PasswordHash = HashPassword(model.Password)
			};
			try
			{
				await _userRepository.CreateAsync(user);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while registering the user.");
			}

			return RedirectToAction("Index", "Home");
		}

		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}
	}
}
