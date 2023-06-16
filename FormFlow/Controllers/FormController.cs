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
		public IActionResult Display()
		{
			return View();
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
				ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value),
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

			formViewModel.Form.OwnerId = idClaim.Value;
            formViewModel.Form.Status = Enum.Parse<FormStatus>(status);

			var questions = formViewModel.ListForms.SelectMany(form => form.Questions!).ToList();

			var formDetails = new Form
			{
				Title = formViewModel.Form.Title,
				Questions = questions,
                Status = formViewModel.Form.Status,
                OwnerId = formViewModel.Form.OwnerId
			};
            foreach (var question in questions)
            {
                _dbContext.Questions.Add(question);
            }
			_dbContext.Forms.Add(formDetails);
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
