using FormFlow.Data.Repositories;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Controller]
	[Route("api/[controller]")]
	public class ResponseEntryController : Controller
	{
		private readonly ResponseEntryRepository _responseEntryRepository;

		public ResponseEntryController(ResponseEntryRepository responseEntryRepository)
		{
			_responseEntryRepository = responseEntryRepository;
		}

		[HttpGet]
		public async Task<List<ResponseEntry>> Get()
		{
			return await _responseEntryRepository.GetAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ResponseEntry>> GetOne(string id)
		{
			var responseEntry = await _responseEntryRepository.GetByIdAsync(id);
			if (responseEntry == null)
			{
				return NotFound();
			}
			return Ok(responseEntry);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] ResponseEntry responseEntry)
		{
			await _responseEntryRepository.CreateAsync(responseEntry);
			return CreatedAtAction(nameof(GetOne), new { id = responseEntry.Id }, responseEntry);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] ResponseEntry responseEntry)
		{
			var existingEntry = await _responseEntryRepository.GetByIdAsync(id);
			if (existingEntry == null)
			{
				return NotFound();
			}

			responseEntry.Id = id; // Ensure the ID is preserved
			await _responseEntryRepository.UpdateAsync(responseEntry);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existingEntry = await _responseEntryRepository.GetByIdAsync(id);
			if (existingEntry == null)
			{
				return NotFound();
			}

			await _responseEntryRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
