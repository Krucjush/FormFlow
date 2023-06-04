using FormFlow.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class QuestionRepository
	{
		private readonly IMongoCollection<Question> _questions;

		public QuestionRepository(IOptions<MongoDBSettings> mongoDbSettings)
		{
			var client = new MongoClient(mongoDbSettings.Value.ConnectionURI);
			var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
			_questions = database.GetCollection<Question>(mongoDbSettings.Value.Collections["Question"]);
		}
		
		public async Task<List<Question>> GetAsync()
		{
			return await _questions.Find(new BsonDocument()).ToListAsync();
		}

		public async Task<Question> GetByIdAsync(string id)
		{
			var filter = Builders<Question>.Filter.Eq("Id", id);
			return await _questions.Find(filter).FirstOrDefaultAsync();
		}

		public async Task CreateAsync(Question question)
		{
			await _questions.InsertOneAsync(question);
		}

		public async Task UpdateAsync(Question question)
		{
			var filter = Builders<Question>.Filter.Eq("Id", question.Id);
			await _questions.ReplaceOneAsync(filter, question);
		}

		public async Task DeleteAsync(string id)
		{
			var filter = Builders<Question>.Filter.Eq("Id", id);
			await _questions.DeleteOneAsync(filter);
		}
	}
}
