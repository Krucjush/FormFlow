using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models;
using FormFlow.Models.Enums;
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
		public FormViewModel FormViewModel { get; set; }

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
			var form = _dbContext.Forms.Include(f => f.Questions).ThenInclude(q => q.Options).FirstOrDefault(f => f.Id == id);

			if (form == null)
			{
				return NotFound();
			}
			return View(form);
		}
		[HttpGet]
		public IActionResult Create()
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			FormViewModel = new FormViewModel
			{
				ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList(),
				Form = new Form(),
				Question = new Question(),
				Questions = new List<Question>(),
				Status = FormStatus.Public
			};

			return View(FormViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(FormViewModel formViewModel, string status)
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			formViewModel.ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList();

			var formDetails = new Form
			{
				Title = formViewModel.Form.Title,
				Questions = formViewModel.Form.Questions!.Select(q => new Question
				{
					Text = q.Text,
					Options = q.Options != null ? q.Options.Select(o => new Option { Text = o.Text }).ToList() : new List<Option>(),
					FormId = 0,
					Type = q.Type
				}).ToList(),
				Status = Enum.Parse<FormStatus>(status),
				OwnerId = idClaim.Value
			};

			_dbContext.Forms.Add(formDetails);
			_dbContext.SaveChanges();

			foreach (var question in formDetails.Questions)
			{
				question.FormId = formDetails.Id;
			}

			_dbContext.SaveChanges();

			return RedirectToAction("Index");
		}
		public IActionResult Modify(int formId)
		{
			return View("Index");
		}

		public IActionResult Remove(int formId)
		{
			return View("Index");
		}
	}
}
