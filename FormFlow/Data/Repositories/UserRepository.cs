using FormFlow.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class UserRepository
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository()
		{
			// This constructor is needed for testing or any other scenarios where you don't want to provide the dependencies
		}

		public UserRepository(IOptions<MongoDBSettings> mongoDbSettings)
		{
			var client = new MongoClient(mongoDbSettings.Value.ConnectionURI);
			var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
			_users = database.GetCollection<User>(mongoDbSettings.Value.Collections["User"]);
		}

		public virtual async Task CreateAsync(User user)
		{
			await _users.InsertOneAsync(user);
		}

		public async Task<List<User>> GetAsync()
		{
			return await _users.Find(new BsonDocument()).ToListAsync();
		}
		public async Task<User> GetByEmailAsync(string email)
		{
			return await _users.Find(f => f.Email == email).FirstOrDefaultAsync();
		}
		public async Task<User> GetByIdAsync(string id)
		{
			return await _users.Find(f => f.Id == id).FirstOrDefaultAsync();
		}

		public async Task AddFormAsync(string id, Form form)
		{
			var filter = Builders<User>.Filter.Eq("id", id);
			var update = Builders<User>.Update.AddToSet<Form>("Forms", form);
			await _users.UpdateOneAsync(filter, update);
		}

		public async Task DeleteAsync(string id)
		{
			var filter = Builders<User>.Filter.Eq("Id", id);
			await _users.DeleteOneAsync(filter);
		}
	}
}
