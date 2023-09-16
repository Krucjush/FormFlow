using FormFlow.Data;
using FormFlow.Interfaces;
using FormFlow.Models;
using FormFlow.Models.Enums;
using FormFlow.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Services
{
	public class FormService : IFormService
	{
		private readonly AppDbContext _dbContext;
		private readonly UserManager<User> _userManager;

		public FormService(AppDbContext dbContext, UserManager<User> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<List<Form>> GetFormsAsync()
		{
			return await _dbContext.Forms.ToListAsync();
		}

		public async Task<Form?> GetFormByIdAsync(int id)
		{
			return await _dbContext.Forms.FirstOrDefaultAsync(q => q.Id == id);
		}

		public async Task<List<Form>> GetUserFormsAsync(string userId)
		{
			return await _dbContext.Forms.Where(q => q.OwnerId == userId).ToListAsync();
		}

		public async Task<Form> GetFormDetailsAsync(int id)
		{
			var form = await _dbContext.Forms
				.Include(f => f.Questions)!
				.ThenInclude(q => q.Options)
				.FirstOrDefaultAsync(f => f.Id == id);

			return form ?? null;
		}
	}
}
