using FormFlow.Data.Repositories;
using FormFlow.JWT;
using FormFlow.Models;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Controllers
{
	[Controller]
	[Route("[controller]")]
	public class FormController : Controller
	{
		private readonly FormRepository _formRepository;
		private readonly JwtHelper _jwtHelper;

		public FormController(FormRepository formRepository, JwtHelper jwtHelper)
		{
			_formRepository = formRepository;
            _jwtHelper = jwtHelper;
        }
		

		[HttpGet]
		public async Task<List<Form>> Get()
		{
			return await _formRepository.GetAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Form>> GetOne(string id)
		{
			var form = await _formRepository.GetByIdAsync(id);
			if (form == null)
			{
				return NotFound();
			}
			return Ok(form);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Form form)
		{
			await _formRepository.CreateAsync(form);
			return CreatedAtAction(nameof(GetOne), new { id = form.Id }, form);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] Form form)
		{
			var existingForm = await _formRepository.GetByIdAsync(id);
			if (existingForm == null)
			{
				return NotFound();
			}

			if (await _formRepository.HasResponsesAsync(id))
			{
				// If the form has response, create a new form instead of updating
				form.Id = null; // Reset the ID to generate a new one
				await _formRepository.CreateAsync(form);
				return CreatedAtAction(nameof(GetOne), new { id = form.Id }, form);
			}

			//Update the existing form
			form.Id = id; // Ensure the ID is preserved
			await _formRepository.UpdateAsync(form);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existingForm = await _formRepository.GetByIdAsync(id);
			if (existingForm == null)
			{
				return NotFound();
			}

			if (await _formRepository.HasResponsesAsync(id))
			{
				// If the form has responses, return an error
				return BadRequest("Cannot delete a form with existing responses.");
			}
			await _formRepository.DeleteAsync(id);
			return NoContent();
		}

		[HttpGet("create")]
        // GET: Form/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
		public async Task<IActionResult> Create(FormCreateModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _jwtHelper.GetUserIdFromJwt(Request.Headers["Authorization"]!);

            var form = new Form()
            {
                Title = model.Title,
                Questions = model.Questions,
                Status = model.Status ?? FormStatus.Public,
                OwnerId = userId
            };
            await _formRepository.CreateAsync(form);

            return RedirectToAction("Index", "Home");
        }
		
	}
}
