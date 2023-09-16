using System.Security.Claims;
using FormFlow.Interfaces;
using FormFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Authorize]
	[Route("api/forms/user")]
	[ApiController]
	public class UserFormsApiController : ControllerBase
	{
		private readonly IFormService _formService;

		public UserFormsApiController(IFormService formService)
		{
			_formService = formService;
		}

		[HttpGet]
		public async Task<ActionResult<List<Form>>> GetUserForms()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return Unauthorized();
			}

			var forms = await _formService.GetUserFormsAsync(userId);

			return Ok(forms);
		}
	}
}
