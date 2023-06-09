using FormFlow.Models;

namespace FormFlow.Interfaces
{
	public interface IFormRepository
	{
		public Task CreateAsync(Form form);
		public Task<List<Form>> GetAsync();
		public Task<Form> GetByIdAsync(string id);
		public Task UpdateAsync(Form form);
		public Task DeleteAsync(string id);
		public Task<bool> HasResponsesAsync(string formId);
		public Task DisassociateFormsFromUserAsync(string userId);
	}
}
