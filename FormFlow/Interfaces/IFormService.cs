using FormFlow.Models;
using FormFlow.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Interfaces
{
	public interface IFormService
	{
		Task<List<Form>> GetFormsAsync();
		Task<Form?> GetFormByIdAsync(int id);
		Task<List<Form>> GetUserFormsAsync(string userId);
		Task<Form> GetFormDetailsAsync(int id);
	}
}
