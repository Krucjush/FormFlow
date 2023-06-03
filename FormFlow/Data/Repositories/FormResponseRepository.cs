using FormFlow.Models;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class FormResponseRepository
	{
		private readonly IMongoCollection<FormResponse> _formResponses;

		public FormResponseRepository(MongoDbContext dbContext)
		{
			_formResponses = dbContext.FormResponse;
		}

		public void Create(FormResponse form)
		{
			_formResponses.InsertOne(form);
		}

		public FormResponse GetById(string id)
		{
			return _formResponses.Find(f => f.Id == id).FirstOrDefault();
		}

		public void Update(FormResponse formResponse)
		{
			_formResponses.ReplaceOne(f => f.Id == formResponse.Id, formResponse);
		}

		public void Delete(string id)
		{
			_formResponses.DeleteOne(f => f.Id == id);
		}
	}
}
