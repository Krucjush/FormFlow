using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Controller]
	[Route("api/[controller]")]
	public class QuestionController : Controller
	{
		private readonly QuestionRepository _questionRepository;

		public QuestionController(QuestionRepository questionRepository)
		{
			_questionRepository = questionRepository;
		}

		[HttpGet]
		public async Task<List<Question>> Get()
		{
			return await _questionRepository.GetAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Question>> GetOne(string id)
		{
			var question = await _questionRepository.GetByIdAsync(id);
			if (question == null)
			{
				return NotFound();
			}
			return Ok(question);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Question question)
		{
			await _questionRepository.CreateAsync(question);
			return CreatedAtAction(nameof(GetOne), new { id = question.Id }, question);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] Question question)
		{
			var existingQuestions = await _questionRepository.GetByIdAsync(id);
			if (existingQuestions == null)
			{
				return NotFound();
			}

			question.Id = id; // Ensure the ID is preserved
			await _questionRepository.UpdateAsync(question);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existingQuestion = await _questionRepository.GetByIdAsync(id);
			if (existingQuestion == null)
			{
				return NotFound();
			}

			await _questionRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
