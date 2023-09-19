using FormFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : Controller
	{
		[HttpGet("login")]
		public IActionResult Login()
		{
			var token = JwtTokenService.GenerateJwtToken("ConfirmedUser@example.com");

			HttpContext.Session.SetString("JwtToken", token);

			return Ok("zalogowano");
		}
	}
}
