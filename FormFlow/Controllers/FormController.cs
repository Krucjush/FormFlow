using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models;
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
				Questions = new List<Question>()
			};

			return View(FormViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(FormViewModel formViewModel)
		{
			var claims = User.Identity as ClaimsIdentity;
			var idClaim = claims?.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim == null)
			{
				return Unauthorized();
			}

			FormViewModel.ListForms = _dbContext.Forms.Include(f => f.Questions).Where(f => f.OwnerId == idClaim.Value).ToList();

			FormViewModel.Form.OwnerId = idClaim.Value;

			var questions = FormViewModel.ListForms.SelectMany(form => form.Questions).ToList();

			var formDetails = new Form
			{
				Title = FormViewModel.Form.Title,
				Status = FormViewModel.Form.Status,
				Questions = questions,
				OwnerId = FormViewModel.Form.OwnerId
			};
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
