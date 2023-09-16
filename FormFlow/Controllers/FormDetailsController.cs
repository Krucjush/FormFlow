using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Interfaces;
using FormFlow.Models;
using FormFlow.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Authorize]
	[Route("api/forms/{formId}")]
	[ApiController]
	public class FormDetailsController : Controller
	{
		private readonly IFormService _formService;
		private readonly FormFlowContext _formFlowContext;

		public FormDetailsController(IFormService formService, FormFlowContext formFlowContext)
		{
			_formService = formService;
			_formFlowContext = formFlowContext;
		}

		[HttpGet]
		public async Task<IActionResult> GetFormDetail(int formId)
		{
			var claims = User.Identity as ClaimsIdentity;
			var user = _formFlowContext.Users.FirstOrDefault(u => u.Email == claims!.Name);

			if (user == null)
			{
				return Unauthorized();
			}

			var form = await _formService.GetFormDetailsAsync(formId);

			return RedirectToAction("Display","Form", form);
		}
	}
}
