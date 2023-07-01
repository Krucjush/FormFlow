using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models;
using FormFlow.Models.Enums;
using FormFlow.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Controllers
{
	public class FormController : Controller
	{
		private readonly AppDbContext? _dbContext;
		private readonly FormFlowContext? _formFlowContext;
		private readonly UserManager<User>? _userManager;
		public int FormId { get; set; }
		[BindProperty] 
		public FormViewModel? FormViewModel { get; set; }

		public FormController(AppDbContext? dbContext, FormFlowContext? formFlowContext, UserManager<User>? userManager)
		{
			_dbContext = dbContext;
			_formFlowContext = formFlowContext;
			_userManager = userManager;
		}
		[Authorize]
		public IActionResult Index(string? errorMessage = null)
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			FormViewModel = new FormViewModel
			{
				ListForms = _dbContext!.Forms.Include(f => f.Questions).Where(q => q.OwnerId == idClaim.Value)
			};

			ViewData["ErrorMessage"] = errorMessage;

			return View(FormViewModel);
		}
		[HttpGet]
		public async Task<IActionResult> Display(int id)
		{
			var form = _dbContext!.Forms.Include(f => f.Questions!).ThenInclude(q => q.Options).FirstOrDefault(f => f.Id == id);

			if (form == null)
			{
				return NotFound();
			}

			var claims = User.Identity as ClaimsIdentity;
			var user = _formFlowContext!.Users.FirstOrDefault(u => u.Email == claims!.Name);
			var isEmailConfirmed = await _userManager!.IsEmailConfirmedAsync(user!);

			if (CanSubmitResponse(form, user?.Email!, isEmailConfirmed)) return View(form);
			var errorMessage = $"You don't have permission to contribute to this form.\nForm Status is {form.Status}.\n";
			switch (form.Status)
			{
				case FormStatus.Private:
					errorMessage += "Please confirm your email to contribute to this form.";
					break;
				case FormStatus.Domain:
					errorMessage += $"Form domain is `{GetFromDomain(form)}`.\nYour domain is `{GetUserEmailDomain(user!.Email!)}`.";
					break;
			}
			return RedirectToAction("Index", "Home", new { errorMessage });
		}
		[Authorize]
		[HttpGet]
		public IActionResult Create()
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			FormViewModel ??= new FormViewModel
			{
				ListForms = _dbContext!.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList(),
				Form = new Form
				{
					Questions = new List<Question>()
				},
				Question = new Question(),
				Questions = new List<Question>(),
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			return View(FormViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(FormViewModel formViewModel, string status, string type)
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			if (formViewModel.Form?.Questions?.Count < 1)
			{
				return BadRequest("At lest one question is required.");
			}

			formViewModel.ListForms = _dbContext!.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList();
			
			var typeArray = type.Split(',').Select(t => t.Trim()).ToList();

			var formDetails = new Form
			{
				Title = formViewModel.Form!.Title,
				Questions = formViewModel.Form.Questions!.Select((q, index) => new Question
				{
					Text = q.Text,
					Options = q.Options != null ? q.Options.Select(o => new Option { Text = o.Text }).ToList() : new List<Option>(),
					FormId = 0,
					Type = Enum.Parse<QuestionType>(typeArray[index])
				}).ToList(),
				Status = Enum.Parse<FormStatus>(status),
				OwnerId = idClaim.Value
			};

			if (formDetails.Questions.Any(formDetailsQuestion => formDetailsQuestion.Type is QuestionType.MultipleOptions && formDetailsQuestion.Options!.Count < 1))
			{
				return BadRequest("At least one option is required for a MultipleOptions question.");
			}

			_dbContext.Forms.Add(formDetails);
			_dbContext.SaveChanges();

			foreach (var question in formDetails.Questions)
			{
				question.FormId = formDetails.Id;
			}

			_dbContext.SaveChanges();

			return RedirectToAction("Index");
		}
		[Authorize]
		[HttpGet]
		public IActionResult Modify(int formId)
		{
			FormId = formId;
			var form = _dbContext!.Forms
				.Include(f => f.Questions)!
				.ThenInclude(q => q.Options)
				.FirstOrDefault(f => f.Id == formId);

			if (form == null)
			{
				return NotFound();
			}

			var formViewModel = new FormViewModel
			{
				Form = new Form
				{
					Id = formId,
					Title = form.Title,
					Questions = form.Questions!.Select(q => new Question
					{
						Text = q.Text,
						Options = q.Options!.Select(o => new Option
						{
							Text = o.Text
						}).ToList(),
						Type = q.Type
					}).ToList(),
					Status = form.Status,
					OwnerId = form.OwnerId
				},
				Status = form.Status,
				QuestionTypes = Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().ToList()
			};
			ViewBag.FormHasResponses = FormHasResponses(formId);
			return View(formViewModel);
		}
		[ValidateAntiForgeryToken]
		[HttpPatch]
		public IActionResult Modify(FormViewModel formViewModel, string status, string? type)
		{
			
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);

			if (idClaim == null)
			{
				return Unauthorized();
			}

			var currentForm = _dbContext!.Forms
				.Include(f => f.Questions)!
				.ThenInclude(q => q.Options)
				.FirstOrDefault(f => f.Id == formViewModel.Form!.Id);

			if (formViewModel.Form?.Questions?.Count < 1)
			{
				return BadRequest("At lest one question is required.");
			}

			formViewModel.ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList();
			var typeArray = new List<string>();
			if (type != null)
			{
				typeArray = type.Split(',').Select(t => t.Trim()).ToList();
			}
			var diff = formViewModel.Form!.Questions!.Count - typeArray.Count;

			var formDetails = new Form
			{
				Id = formViewModel.Form.Id,
				Title = formViewModel.Form.Title,
				Questions = formViewModel.Form.Questions!.Select((q, index) => new Question
				{
					Id = q.Id,
					Text = q.Text,
					Options = q.Options != null ? q.Options.Select(o => new Option { Id = o.Id, Text = o.Text }).ToList() : new List<Option>(),
					FormId = 0,
					Type = q.Type ?? Enum.Parse<QuestionType>(typeArray[index - diff])
				}).ToList(),
				Status = Enum.Parse<FormStatus>(status),
				OwnerId = idClaim.Value
			};

			if (formDetails.Questions.Any(formDetailsQuestion => formDetailsQuestion.Type is QuestionType.MultipleOptions && formDetailsQuestion.Options!.Count < 1))
			{
				return BadRequest("At least one option is required for a MultipleOptions question.");
			}

			if (FormHasResponses(formDetails.Id))
			{
				var newFormDetails = new Form
				{
					Title = formViewModel.Form.Title,
					Questions = formViewModel.Form.Questions!.Select((q, index) => new Question
					{
						Id = q.Id,
						Text = q.Text,
						Options = q.Options != null
							? q.Options.Select(o => new Option { Id = o.Id, Text = o.Text }).ToList()
							: new List<Option>(),
						FormId = 0,
						Type = q.Type ?? Enum.Parse<QuestionType>(typeArray[index - diff])
					}).ToList(),
					Status = Enum.Parse<FormStatus>(status),
					OwnerId = idClaim.Value
				};

				_dbContext.Forms.Add(newFormDetails);
				_dbContext.SaveChanges();

				foreach (var question in newFormDetails.Questions)
				{
					question.FormId = newFormDetails.Id;
				}

				_dbContext.SaveChanges();

				return RedirectToAction("Index");
			}

			_dbContext.Forms.Remove(currentForm!);
			_dbContext.Forms.Update(formDetails);
			_dbContext.SaveChanges();

			foreach (var question in formDetails.Questions)
			{
				question.FormId = formDetails.Id;
			}

			_dbContext.SaveChanges();

			return RedirectToAction("Index");
		}

		public bool FormHasResponses(int formId)
		{
			return _dbContext!.FormResponses.Any(fr => fr.FormId == formId);
		}

		public IActionResult Remove(int formId)
		{
			var form = _dbContext!.Forms.FirstOrDefault(f => f.Id == formId);

			if (form == null)
			{
				return NotFound();
			}

			if (FormHasResponses(formId))
			{
				const string errorMessage = "Cannot remove the form as it has associated responses.";
				return RedirectToAction("Index", new { errorMessage });
			}

			_dbContext.Forms.Remove(form);
			_dbContext.SaveChanges();

			return RedirectToAction("Index");
		}
		public virtual bool CanSubmitResponse(Form form, string userEmail, bool isAuthenticated)
		{
			switch (form.Status)
			{
				case FormStatus.Public:
					return true;

				case FormStatus.Private:
					return isAuthenticated;

				case FormStatus.Domain:
					if (string.IsNullOrEmpty(userEmail))
					{
						return false;
					}

					var formDomain = GetFromDomain(form);
					var userEmailDomain = GetUserEmailDomain(userEmail);
					return formDomain.Equals(userEmailDomain, StringComparison.OrdinalIgnoreCase);
				default:
					return false;
			}
		}

		public virtual string GetFromDomain(Form form)
		{
			var owner = _formFlowContext!.Users.FirstOrDefault(u => u.Id == form.OwnerId);
			if (owner == null) return string.Empty;
			var emailAddress = owner.Email;
			var domain = emailAddress!.Split('@')[1];
			return domain;
		}

		public virtual string GetUserEmailDomain(string userEmail)
		{
			var emailParts = userEmail.Split('@');
			return emailParts.Length > 1 ? emailParts[1] : string.Empty;
		}
	}
}
