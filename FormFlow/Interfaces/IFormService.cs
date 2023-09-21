using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Interfaces
{
	public interface IFormService
	{
		Task<List<Form>> GetFormsAsync();
		Task<Form?> GetFormByIdAsync(int id);
		Task<bool> SaveFormAsync(Form form);
		Task<int> DeleteFormAsync(int id);
		Task<int> UpdateFormAsync(Form? form, string token);
		Task<bool> SaveQuestionAsync(int id, Question question);
		Task<bool> DeleteQuestionAsync(int id);
		Task<bool> UpdateQuestionAsync(Question? question);
		Task<bool> SaveAnswerAsync(int id, ResponseEntry responseEntry, string email);
		Task<List<ResponseEntry>> GetFormResponsesByForm(Form form);
	}
}
