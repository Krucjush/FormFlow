using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Controllers
{
    public class FormController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

		public FormController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new Form
            {
                Questions = new List<Question>()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Form model)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var user = _dbContext.Users.FirstOrDefaultAsync(u => u.Email == identityUser!.Email);
            model.OwnerId = user.Id;
            if (!ModelState.IsValid)
            {
	            foreach (var (propertyName, value) in ModelState)
	            {
		            var errorMessages = value.Errors.Select(e => e.ErrorMessage); // List of error messages

		            foreach (var errorMessage in errorMessages)
		            {
			            Console.WriteLine($"Validation error: {propertyName} - {errorMessage}");
		            }
	            }
				return View(model);
            }

            var questions = model.Questions.Select(q => new Question
            {
                Text = q.Text,
                Type = q.Type,
                Options = q.Options?.Select(o => new Option { Text = o.Text }).ToList(),
            }).ToList();

            var form = new Form
            {
                Title = model.Title,
                Questions = questions,
                Status = model.Status
            };

            foreach (var question in form.Questions)
            {
                question.FormId = form.Id;
            }

            _dbContext.Forms.Add(form);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
