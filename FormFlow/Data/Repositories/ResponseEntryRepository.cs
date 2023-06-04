using FormFlow.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class ResponseEntryRepository
	{
		private readonly IMongoCollection<ResponseEntry> _responseEntries;

		public ResponseEntryRepository(IOptions<MongoDBSettings> mongoDbSettings)
		{
			var client = new MongoClient(mongoDbSettings.Value.ConnectionURI);
			var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
			_responseEntries = database.GetCollection<ResponseEntry>(mongoDbSettings.Value.Collections["ResponseEntry"]);
		}

		public async Task<List<ResponseEntry>> GetAsync()
		{
			return await _responseEntries.Find(new BsonDocument()).ToListAsync();
		}

		public async Task<ResponseEntry> GetByIdAsync(string id)
		{
			var filter = Builders<ResponseEntry>.Filter.Eq("Id", id);
			return await _responseEntries.Find(filter).FirstOrDefaultAsync();
		}

		public async Task CreateAsync(ResponseEntry responseEntry)
		{
			await _responseEntries.InsertOneAsync(responseEntry);
		}

		public async Task UpdateAsync(ResponseEntry responseEntry)
		{
			var filter = Builders<ResponseEntry>.Filter.Eq("Id", responseEntry.Id);
			await _responseEntries.ReplaceOneAsync(filter, responseEntry);
		}

		public async Task DeleteAsync(string id)
		{
			var filter = Builders<ResponseEntry>.Filter.Eq("Id", id);
			await _responseEntries.DeleteOneAsync(filter);
		}
	}
}
