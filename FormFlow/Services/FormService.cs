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
			return await _dbContext.Forms
				.Include(i => i.Questions!)
				.ThenInclude(t => t.Options)
				.ToListAsync();
		}

		public async Task<Form?> GetFormByIdAsync(int id)
		{
			return await _dbContext.Forms
				.Include(i => i.Questions!)
				.ThenInclude(t => t.Options)
				.FirstOrDefaultAsync(q => q.Id == id);
		}

		public async Task<bool> SaveFormAsync(Form form)
		{
			_dbContext.Add(form);
			return await _dbContext.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteFormAsync(int id)
		{
			var form = await _dbContext.Forms.FindAsync(id);

			if (form == null)
			{
				return false;
			}
			else
			{
				_dbContext.Forms.Remove(form);
				return await _dbContext.SaveChangesAsync() > 0;
			}
		}

		public async Task<bool> UpdateFormAsync(Form? form)
		{
			var formToUpdate = await _dbContext.Forms.FindAsync(form?.Id);

			if (formToUpdate == null)
			{
				return false;
			}
			formToUpdate.Title = form!.Title;
			formToUpdate.Status = form.Status;
			formToUpdate.OwnerId = form.OwnerId;
			formToUpdate.Questions = form.Questions;

			return await _dbContext.SaveChangesAsync() > 0;
		}

		public async Task<bool> SaveQuestionAsync(int id, Question question)
		{
			var form = await _dbContext.Forms.FindAsync(id);

			if (form == null)
			{
				return false;
			}
			question.FormId = id;
			_dbContext.Add(question);

			return await _dbContext.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteQuestionAsync(int id)
		{
			var question = await _dbContext.Questions.FindAsync(id);

			if (question == null)
			{
				return false;
			}

			_dbContext.Questions.Remove(question);
			return await _dbContext.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateQuestionAsync(Question? question)
		{
			var questionToUpdate = await _dbContext.Questions.FindAsync(question?.Id);

			if (questionToUpdate == null)
			{
				return false;
			}

			questionToUpdate.FormId = question!.FormId;
			questionToUpdate.Text = question.Text;
			questionToUpdate.Type = question.Type;
			questionToUpdate.Options = question.Options;
			questionToUpdate.Required = question.Required;
			questionToUpdate.MultipleChoice = question.MultipleChoice;

			return await _dbContext.SaveChangesAsync() > 0;
		}
	}
}
