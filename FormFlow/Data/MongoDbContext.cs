using FormFlow.Models;
using MongoDB.Driver;

namespace FormFlow.Data
{
	// ReSharper disable once IdentifierTypo
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		// ReSharper disable once IdentifierTypo
		public MongoDbContext(string connectionString, string databaseName)
		{
			var client = new MongoClient(connectionString);
			_database = client.GetDatabase(databaseName);
		}

		public IMongoCollection<Form> Forms => _database.GetCollection<Form>("Forms");
		public IMongoCollection<FormResponse> FormResponse => _database.GetCollection<FormResponse>("FormResponses");
		public IMongoCollection<Question> Questions => _database.GetCollection<Question>("Questions");
		public IMongoCollection<ResponseEntry> ResponseEntries => _database.GetCollection<ResponseEntry>("ResponseEntries");
		public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
	}
}
