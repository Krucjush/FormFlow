using FormFlow.Data;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;

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

		public async Task<IActionResult> Index()
		{
			var forms = await _dbContext.Forms.ToListAsync();
			return View(forms);
		}

		public IActionResult Privacy()
		{
			return View();
		}
	}
}