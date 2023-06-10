using System.IdentityModel.Tokens.Jwt;
using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FormFlow.Interfaces;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Identity;
using FormFlow.JWT;

namespace FormFlow.Controllers
{
    [Controller]
	[Route("[controller]")]
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IFormRepository _formRepository;
		private readonly JwtSettings _jwtSettings;

		public UserController(IUserRepository userRepository, IFormRepository formRepository, JwtSettings jwtSettings)
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
		public IActionResult RegisterAndLogin()
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

			if (!IsValidPassword(model.Password))
			{
				ModelState.AddModelError("Password", "Password does not meet the requirements");
				return BadRequest(ModelState);
			}

			if (!IsValidEmail(model.Email))
			{
				ModelState.AddModelError("Email", "Invalid email format.");
				return BadRequest(ModelState);
			}

			if (model.Password != model.ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password do not match.");
				return BadRequest(ModelState);
			}

			if (await _userRepository.ExistsByEmailAsync(model.Email))
			{
				ModelState.AddModelError("Email", "Email address already used.");
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
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while registering the user.");
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(UserLoginModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = await _userRepository.GetByEmailAsync(model.Email);

			if (user == null)
			{
				ModelState.AddModelError("Email", "Invalid email or password.");
				return BadRequest(ModelState);
			}
			if (VerifyPassword(model.Password, user.PasswordHash)) return RedirectToAction("Index", "Home");
			ModelState.AddModelError("Email", "Invalid email or password.");
			return BadRequest(ModelState);
		}

		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		private static bool IsValidPassword(string password)
		{
			const int minimumPasswordLength = 8;

			if (string.IsNullOrEmpty(password) || password.Length < minimumPasswordLength)
			{
				return false;
			}

			var hasUppercase = false;
			var hasLowercase = false;
			var hasDigit = false;
			var hasSpecialCharacter = false;

			foreach (var c in password)
			{
				if (char.IsUpper(c))
				{
					hasUppercase = true;
				}
				else if (char.IsLower(c))
				{
					hasLowercase = true;
				}
				else if (char.IsDigit(c))
				{
					hasDigit = true;
				}
				else if (char.IsSymbol(c) || char.IsPunctuation(c))
				{
					hasSpecialCharacter = true;
				}
			}

			const int requiredCharacterTypes = 2;

			return (hasLowercase && hasUppercase ? 1 : 0) + (hasDigit || hasSpecialCharacter ? 1 : 0) >= requiredCharacterTypes;
		}

		private static bool IsValidEmail(string email)
		{
			const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

			var regex = new Regex(emailPattern);

			return !string.IsNullOrEmpty(email) && regex.IsMatch(email);
		}

		private bool VerifyPassword(string password, string passwordHash)
		{
			return BCrypt.Net.BCrypt.Verify(password, passwordHash);
		}
	}
}
