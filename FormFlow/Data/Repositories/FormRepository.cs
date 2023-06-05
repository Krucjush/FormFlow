using FormFlow.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class FormRepository
	{
		private readonly IMongoCollection<Form> _forms;
		private readonly IMongoCollection<FormResponse> _formsResponses;

		public FormRepository(IOptions<MongoDBSettings> mongoDbSettings)
		{
			var client = new MongoClient(mongoDbSettings.Value.ConnectionURI);
			var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
			_forms = database.GetCollection<Form>(mongoDbSettings.Value.Collections["Form"]);
			_formsResponses = database.GetCollection<FormResponse>(mongoDbSettings.Value.Collections["FormResponse"]);
		}

		public async Task CreateAsync(Form form)
		{
			await _forms.InsertOneAsync(form);
		}

		public async Task<List<Form>> GetAsync()
		{
			return await _forms.Find(new BsonDocument()).ToListAsync();
		}

		public async Task<Form> GetByIdAsync(string id)
		{
			return await _forms.Find(f => f.Id == id).FirstOrDefaultAsync();
		}

		public async Task UpdateAsync(Form form)
		{
			await _forms.ReplaceOneAsync(f => f.Id == form.Id, form);
		}

		public async Task DeleteAsync(string id)
		{
			await _forms.DeleteOneAsync(f => f.Id == id);
		}

		public async Task<bool> HasResponsesAsync(string formId)
		{
			var count = await _formsResponses.CountDocumentsAsync(r => r.FormId == formId);
			return count > 0;
		}

		public async Task DisassociateFormsFromUserAsync(string userId)
		{
			var filter = Builders<Form>.Filter.Eq("OwnerId", userId);
			var update = Builders<Form>.Update.Set<string?>("OwnerId", null); // Set the OwnerId to null
			await _forms.UpdateManyAsync(filter, update);
		}
	}
}
