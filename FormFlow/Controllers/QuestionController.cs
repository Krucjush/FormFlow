using System.Text.Json;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	public class QuestionController : Controller
	{
		[HttpGet]
		public IActionResult GetQuestionTypes()
		{
			var questionTypes = Enum.GetNames(typeof(QuestionType)).ToList();
			return Json(questionTypes);
		}
	}
}
