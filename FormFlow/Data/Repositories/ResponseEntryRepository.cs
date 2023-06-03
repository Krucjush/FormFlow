using FormFlow.Models;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class ResponseEntryRepository
	{
		private readonly IMongoCollection<ResponseEntry> _responseEntries;

		public ResponseEntryRepository(MongoDbContext dbContext)
		{
			_responseEntries = dbContext.ResponseEntries;
		}

		public void Create(ResponseEntry responseEntry)
		{
			_responseEntries.InsertOne(responseEntry);
		}

		public ResponseEntry GetById(string id)
		{
			return _responseEntries.Find(f => f.Id == id).FirstOrDefault();
		}

		public void Update(ResponseEntry responseEntry)
		{
			_responseEntries.ReplaceOne(f => f.Id == responseEntry.Id, responseEntry);
		}

		public void Delete(string id)
		{
			_responseEntries.DeleteOne(f => f.Id == id);
		}
	}
}
