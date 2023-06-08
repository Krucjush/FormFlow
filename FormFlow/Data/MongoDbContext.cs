using FormFlow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FormFlow.Data
{
	// ReSharper disable once IdentifierTypo
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;
		private MongoClient _mongoClient { get; set; }
		public IClientSessionHandle Session { get; set; }

		// ReSharper disable once IdentifierTypo
		public MongoDbContext(IOptions<MongoDBSettings> configuration)
		{
			_mongoClient = new MongoClient(configuration.Value.ConnectionURI);

			_database = _mongoClient.GetDatabase(configuration.Value.DatabaseName);
		}

		public IMongoCollection<T> GetCollections<T>(string name)
		{
			return string.IsNullOrEmpty(name) ? null : _database.GetCollection<T>(name);
		}
	}
}
