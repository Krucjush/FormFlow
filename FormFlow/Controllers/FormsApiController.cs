using FormFlow.Interfaces;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Identity;
using FormFlow.Services;

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
		public async Task<ActionResult<List<FormDisplayModel>>> GetForms()
		{
			var forms = await _formService.GetFormsAsync();

			return Ok(forms.Select(s => new FormDisplayModel(s)).ToList());
		}

		[HttpGet("display/{id}")]
		[System.Web.Http.Authorize]
		public async Task<IActionResult> GetForm(int id)
		{
			var form = await _formService.GetFormByIdAsync(id);

			foreach (var q in form!.Questions!)
			{
				q.Form = null;

				foreach (var o in q!.Options!)
				{
					o.Question = null;
				}
			}

			var token = HttpContext.Session.GetString("JwtToken");

			return await IsUserAuthorizedToAccessFormByToken(form, token!) == false ? StatusCode(403, "You do not have permission to access this form.") : Ok(form);
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateForm(Form? form)
		{
			if (await _formService.SaveFormAsync(form))
			{
				return Ok("New form added");
			}
			else
			{
				return BadRequest("Something went wrong");
			}
		}

		[HttpPut("update")]
		public async Task<IActionResult> UpdateForm(Form? form)
		{
			if (await _formService.UpdateFormAsync(form))
			{
				return Ok("Form modified");
			}
			else
			{
				return BadRequest("Something went wrong");
			}
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteForm(int id)
		{
			if (await _formService.DeleteFormAsync(id))
			{
				return Ok("Form deleted");
			}
			else
			{
				return BadRequest("Something went wrong");
			}
		}

		//QUESTIONS

		[HttpPost("createquestion/{id}")]
		public async Task<IActionResult> CreateQuestion(int id, Question question)
		{
			if (await _formService.SaveQuestionAsync(id, question))
			{
				return Ok("New question added");
			}
			else
			{
				return BadRequest("Something went wrong");
			}
		}

		[HttpPut("updatequestion")]
		public async Task<IActionResult> UpdateQuestion(Question? question)
		{
			if (await _formService.UpdateQuestionAsync(question))
			{
				return Ok("Question modified");
			}
			else
			{
				return BadRequest("Something went wrong");
			}
		}

		[HttpDelete("deletequestion/{id}")]
		public async Task<IActionResult> DeleteQuestion(int id)
		{
			if (await _formService.DeleteQuestionAsync(id))
			{
				return Ok("Question deleted");
			}
			else
			{
				return BadRequest("Something went wrong");
			}
		}


		private async Task<bool> IsUserAuthorizedToAccessFormByToken(Form form, string token)
		{
			var userName = JwtTokenService.ReadUserFromToken(token);
			var user = _formFlowContext.Users.FirstOrDefault(u => u.Email == userName);

			if (form.Status == FormStatus.Private && await _userManager.IsEmailConfirmedAsync(user!) == false)
			{
				return false;
			}

			var owner = _formFlowContext.Users.FirstOrDefault(u => u.Id == form.OwnerId);
			var formDomain = owner!.Email!.Split('@')[1];
			var userDomain = user!.Email!.Split('@')[1];

			return form.Status != FormStatus.Domain || formDomain.Equals(userDomain, StringComparison.OrdinalIgnoreCase);
		}


		//      private async Task<bool> IsUserAuthorizedToAccessForm(Form form)
		//{
		//	var claims = User.Identity as ClaimsIdentity;
		//	var user = _formFlowContext.Users.FirstOrDefault(u => u.Email == claims!.Name);
		//	if (form.Status == FormStatus.Private && await _userManager.IsEmailConfirmedAsync(user!) == false)
		//	{
		//		return false;
		//	}

		//	var owner = _formFlowContext.Users.FirstOrDefault(u => u.Id == form.OwnerId);
		//	var formDomain = owner!.Email!.Split('@')[1];
		//	var userDomain = user!.Email!.Split('@')[1];

		//	return form.Status != FormStatus.Domain || formDomain.Equals(userDomain, StringComparison.OrdinalIgnoreCase);
		//}
	}
}
