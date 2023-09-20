using FormFlow.Models;
using FormFlow.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly UserManager<User> _userManager;

		public UsersController(UserManager<User> userManager)
		{
			_userManager = userManager;
		}

		[HttpGet("login/{userName}/{password}")]
		public async Task<IActionResult> Login(string userName, string password)
		{
			var user = await _userManager.FindByNameAsync(userName);

			if (await _userManager.CheckPasswordAsync(user, password))
			{
				var token = JwtTokenService.GenerateJwtToken(userName);

				HttpContext.Session.SetString("JwtToken", token);

				return Ok("Logged in");
			}
			else
			{
				return BadRequest("Wrong credentials");
			}
		}
	}
}
