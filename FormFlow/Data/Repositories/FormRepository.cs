using FormFlow.Models;
using MongoDB.Driver;

namespace FormFlow.Data.Repositories
{
	public class FormRepository
	{
		private readonly IMongoCollection<Form> _forms;

		public FormRepository(MongoDbContext dbContext)
		{
			_forms = dbContext.Forms;
		}

		public void Create(Form form)
		{
			_forms.InsertOne(form);
		}

		public Form GetById(string id)
		{
			return _forms.Find(f => f.Id == id).FirstOrDefault();
		}

		public void Update(Form form)
		{
			_forms.ReplaceOne(f => f.Id == form.Id, form);
		}

		public void Delete(string id)
		{
			_forms.DeleteOne(f => f.Id == id);
		}
	}
}
