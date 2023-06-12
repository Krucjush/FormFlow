using System.Security.Claims;
using FormFlow.Data;
using FormFlow.Models;
using FormFlow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
    public class FormController : Controller
    {
        private readonly AppDbContext _dbContext;

        public FormController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new FormViewModel
            {
                Questions = new List<QuestionViewModel>()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
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
                Status = model.Status,
                OwnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            foreach (var question in form.Questions)
            {
                question.FormId = form.Id;
            }

            _dbContext.Forms.Add(form);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
