using FormFlow.Data;
using FormFlow.Models;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
	}
}