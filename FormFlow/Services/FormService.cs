using FormFlow.Data;
using FormFlow.Interfaces;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Services
{
	public class FormService : IFormService
	{
		private readonly AppDbContext _dbContext;

		public FormService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Form>> GetFormsAsync()
		{
			return await _dbContext.Forms.ToListAsync();
		}

		public async Task<Form?> GetFormByIdAsync(int id)
		{
			return await _dbContext.Forms.FirstOrDefaultAsync(q => q.Id == id);
		}
	}
}
