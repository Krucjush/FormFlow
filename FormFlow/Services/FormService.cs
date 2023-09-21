using FormFlow.Data;
using FormFlow.Interfaces;
using FormFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Services
{
	public class FormService : IFormService
	{
		private readonly AppDbContext _dbContext;
		private readonly FormFlowContext _formFlowContext;
		public FormService(AppDbContext dbContext, FormFlowContext formFlowContext)
		{
			_dbContext = dbContext;
			_formFlowContext = formFlowContext;
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

		public async Task<int> DeleteFormAsync(int id)
		{
			var form = await _dbContext.Forms.FindAsync(id);

			if (form == null)
			{
				return 0;
			}

			if (_dbContext.FormResponses.Any(f => f.FormId == form.Id))
			{
				return 1;
			}

			_dbContext.Forms.Remove(form);
			await _dbContext.SaveChangesAsync();
			return 2;
		}

		public async Task<int> UpdateFormAsync(Form? form, string token)
		{
			if (await IsUserAuthorizedToModifyForm(form, token) == false)
			{
				return 0;
			}
			var formToUpdate = await _dbContext.Forms.FindAsync(form?.Id);

			if (formToUpdate == null)
			{
				return 1;
			}

			if (_dbContext.FormResponses.Any(f => f.FormId == form.Id))
			{
				form.Id = 0;
				foreach (var question in form.Questions)
				{
					question.Id = 0;
					foreach (var option in question.Options)
					{
						option.Id = 0;
					}
				}

				if (await SaveFormAsync(form))
				{
					foreach (var question in form.Questions)
					{
						question.FormId = form.Id;
					}
				}
				else
				{
					return 2;
				}

				await _dbContext.SaveChangesAsync();
				return 3;
			}

			formToUpdate.Title = form!.Title;
			formToUpdate.Status = form.Status;
			formToUpdate.OwnerId = form.OwnerId;
			formToUpdate.Questions = form.Questions;

			

			await _dbContext.SaveChangesAsync();

			return 4;
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

		public async Task<bool> SaveAnswerAsync(int id, ResponseEntry responseEntry, string email)
		{
			var form = await _dbContext.Forms.FindAsync(id);

			if (form == null)
			{
				return false;
			}

			var formResponse = new FormResponse
			{
				FormId = id,
				Email = email
			};
			_dbContext.Add(formResponse);

			await _dbContext.SaveChangesAsync();

			responseEntry.FormResponseId = formResponse.Id;
			_dbContext.Add(responseEntry);

			return await _dbContext.SaveChangesAsync() > 0;
		}

		public async Task<List<ResponseEntry>> GetFormResponsesByForm(Form form)
		{
			var formResponses = await _dbContext.FormResponses.Where(q => q.FormId == form.Id).ToListAsync();

			var responseEntries = new List<ResponseEntry>();

			foreach (var formResponse in formResponses)
			{
				var responseEntry = await _dbContext.ResponseEntries.Where(q => q.FormResponseId == formResponse.Id)
					.ToListAsync();

				responseEntries.AddRange(responseEntry);
			}

			return responseEntries;
		}

		private async Task<bool> IsUserAuthorizedToModifyForm(Form form, string token)
		{
			var userName = JwtTokenService.ReadUserFromToken(token);
			var user = await _formFlowContext.Users.FirstOrDefaultAsync(u => u.Email == userName);

			return form.OwnerId == user.Id;
		}
	}
}
