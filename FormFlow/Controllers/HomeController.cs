using FormFlow.Data;
using FormFlow.Models;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _dbContext;

		public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
		{
			_logger = logger;
			_dbContext = dbContext;
		}

		public async Task<IActionResult> Index(string? errorMessage = null)
		{
			var forms = await _dbContext.Forms.ToListAsync();
			ViewData["ErrorMessage"] = errorMessage;

			return View(forms);
		}

		public IActionResult Privacy()
		{
			return View();
		}
	}
}