using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Interfaces
{
	public interface IFormService
	{
		Task<List<Form>> GetFormsAsync();
		Task<Form?> GetFormByIdAsync(int id);
	}
}
