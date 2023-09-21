using FormFlow.Data;
using FormFlow.Interfaces;
using FormFlow.Models;
using FormFlow.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly FormFlowContext _formFlowContext;

		public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, FormFlowContext formFlowContext)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_formFlowContext = formFlowContext;
		}

		[HttpGet("login/{userName}/{password}")]
		public async Task<IActionResult> Login(string userName, string password)
		{
			var user = await _userManager.FindByNameAsync(userName);

			if (user == null)
			{
				return BadRequest("Wrong credentials");
			}

			if (!await _userManager.CheckPasswordAsync(user, password)) return BadRequest("Wrong credentials");
			var token = JwtTokenService.GenerateJwtToken(userName);

			HttpContext.Session.SetString("JwtToken", token);

			return Ok("Logged in");

		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(UserRegistrationModel model)
		{
			var user = new User { UserName = model.userName, Email = model.userName };
			var result = await _userManager.CreateAsync(user, model.password);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				return Ok("Registered");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return BadRequest();
		}

		[HttpGet("{email}")]
		public async Task<IActionResult> GetByEmail(string email)
		{
			var user = await _formFlowContext.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				return BadRequest("User not found");
			}
			return Ok(user);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			if (!_formFlowContext.Users.Any())
			{
				return BadRequest("No users found");
			}
			return Ok(await _formFlowContext.Users.ToListAsync());
		}
	}
}
