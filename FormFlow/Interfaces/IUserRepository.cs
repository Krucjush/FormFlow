using FormFlow.Models;

namespace FormFlow.Interfaces
{
	public interface IUserRepository
	{
		public Task CreateAsync(User user);
		public Task<List<User>> GetAsync();
		public Task<User> GetByEmailAsync(string email);
		public Task<User> GetByIdAsync(string id);
		public Task AddFormAsync(string id, Form form);
		public Task DeleteAsync(string id);
		public Task<bool> ExistsByEmailAsync(string email);
	}
}
