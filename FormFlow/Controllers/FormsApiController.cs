using FormFlow.Interfaces;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace FormFlow.Controllers
{
	[Route("api/forms")]
	[ApiController]
	public class FormsApiController : ControllerBase
	{
		private readonly IFormService _formService;
		private readonly FormFlowContext _formFlowContext;
		private readonly UserManager<User> _userManager;

		public FormsApiController(IFormService formService, FormFlowContext formFlowContext, UserManager<User> userManager)
		{
			_formService = formService;
			_formFlowContext = formFlowContext;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<ActionResult<List<Form>>> GetForms()
		{
			var forms = await _formService.GetFormsAsync();
			return Ok(forms);
		}

		[HttpGet("GetForm/{id}")]
		public async Task<IActionResult> GetForm(int id)
		{
			var form = await _formService.GetFormByIdAsync(id);

			if (form == null)
			{
				return NotFound();
			}

			return await IsUserAuthorizedToAccessForm(form) == false ? StatusCode(403, "You do not have permission to access this form.") : Ok(form);
		}

		private async Task<bool> IsUserAuthorizedToAccessForm(Form form)
		{
			var claims = User.Identity as ClaimsIdentity;
			var user = _formFlowContext.Users.FirstOrDefault(u => u.Email == claims!.Name);
			if (form.Status == FormStatus.Private && await _userManager.IsEmailConfirmedAsync(user!) == false)
			{
				return false;
			}

			var owner = _formFlowContext.Users.FirstOrDefault(u => u.Id == form.OwnerId);
			var formDomain = owner!.Email!.Split('@')[1];
			var userDomain = user!.Email!.Split('@')[1];

			return form.Status != FormStatus.Domain || formDomain.Equals(userDomain, StringComparison.OrdinalIgnoreCase);
		}
	}
}
