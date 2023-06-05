using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Controller]
	[Route("api/[controller]")]
	public class FormResponseController : Controller
	{
		private readonly FormResponseRepository _formResponseRepository;

		public FormResponseController(FormResponseRepository formResponseRepository)
		{
			_formResponseRepository = formResponseRepository;
		}

		[HttpGet]
		public async Task<List<FormResponse>> Get()
		{
			return await _formResponseRepository.GetAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<FormResponse>> GetOne(string id)
		{
			var response = await _formResponseRepository.GetByIdAsync(id);
			if (response == null)
			{
				return NotFound();
			}
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] FormResponse formResponse)
		{
			await _formResponseRepository.CreateAsync(formResponse);
			return CreatedAtAction(nameof(GetOne), new { id = formResponse.Id }, formResponse);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] FormResponse formResponse)
		{
			var existingResponse = await _formResponseRepository.GetByIdAsync(id);
			if (existingResponse == null)
			{
				return NotFound();
			}

			formResponse.Id = id; // Ensure the ID is preserved
			await _formResponseRepository.UpdateAsync(formResponse);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existingResponse = await _formResponseRepository.GetByIdAsync(id);
			if (existingResponse == null)
			{
				return NotFound();
			}

			await _formResponseRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
