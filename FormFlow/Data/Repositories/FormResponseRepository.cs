using FormFlow.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class FormResponseRepository
	{
		private readonly IMongoCollection<FormResponse> _formResponses;

		public FormResponseRepository(IOptions<MongoDBSettings> mongoDbSettings)
		{
			var client = new MongoClient(mongoDbSettings.Value.ConnectionURI);
			var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
			_formResponses = database.GetCollection<FormResponse>(mongoDbSettings.Value.Collections["FormResponse"]);
		}

		public async Task<List<FormResponse>> GetAsync()
		{
			return await _formResponses.Find(new BsonDocument()).ToListAsync();
		}

		public async Task<FormResponse> GetByIdAsync(string id)
		{
			return await _formResponses.Find(r => r.Id == id).FirstOrDefaultAsync();
		}

		public async Task CreateAsync(FormResponse formResponse)
		{
			await _formResponses.InsertOneAsync(formResponse);
		}

		public async Task UpdateAsync(FormResponse formResponse)
		{
			var filter = Builders<FormResponse>.Filter.Eq(r => r.Id, formResponse.Id);
			await _formResponses.ReplaceOneAsync(filter, formResponse);
		}

		public async Task DeleteAsync(string id)
		{
			var filter = Builders<FormResponse>.Filter.Eq(r => r.Id, id);
			await _formResponses.DeleteOneAsync(filter);
		}
	}
}
