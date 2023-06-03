using FormFlow.Models;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class QuestionRepository
	{
		private readonly IMongoCollection<Question> _questions;

		public QuestionRepository(MongoDbContext dbContext)
		{
			_questions = dbContext.Questions;
		}

		public void Create(Question question)
		{
			_questions.InsertOne(question);
		}

		public Question GetById(string id)
		{
			return _questions.Find(f => f.Id == id).FirstOrDefault();
		}

		public void Update(Question question)
		{
			_questions.ReplaceOne(f => f.Id == question.Id, question);
		}

		public void Delete(string id)
		{
			_questions.DeleteOne(f => f.Id == id);
		}
	}
}
