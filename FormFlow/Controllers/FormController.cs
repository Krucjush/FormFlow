using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models;
using FormFlow.Models.Enums;
using FormFlow.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Controllers
{
	public class FormController : Controller
	{
		private readonly AppDbContext _dbContext;
		[BindProperty] 
		public FormViewModel? FormViewModel { get; set; }

		public FormController(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		[Authorize]
		public IActionResult Index()
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			FormViewModel = new FormViewModel
			{
				ListForms = _dbContext.Forms.Include(f => f.Questions).Where(q => q.OwnerId == idClaim.Value)
			};

			return View(FormViewModel);
		}
		[HttpGet]
		public IActionResult Display(int id)
		{
			var form = _dbContext.Forms.Include(f => f.Questions!).ThenInclude(q => q.Options).FirstOrDefault(f => f.Id == id);

			if (form == null)
			{
				return NotFound();
			}
			return View(form);
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
				ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList(),
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

			formViewModel.ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList();
			
			var typeArray = type.Split(',').Select(t => t.Trim()).ToList();

			var formDetails = new Form
			{
				Title = formViewModel.Form.Title,
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
			var form = _dbContext.Forms
				.Include(f => f.Questions)!
				.ThenInclude(q => q.Options)?
				.FirstOrDefault(f => f.Id == formId);

			if (form == null)
			{
				return NotFound();
			}

			var formViewModel = new FormViewModel
			{
				Form = new Form
				{
					Id = form.Id,
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
				Id = formViewModel.Form!.Id,
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

			_dbContext.Forms.Update(formDetails);
			_dbContext.SaveChanges();

			foreach (var question in formDetails.Questions)
			{
				question.FormId = formDetails.Id;
			}

			_dbContext.SaveChanges();

			return RedirectToAction("Index");
		}

		public IActionResult Remove(int formId)
		{
			return View("Index");
		}
	}
}
