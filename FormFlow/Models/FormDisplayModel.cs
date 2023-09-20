using FormFlow.Models.Enums;

namespace FormFlow.Models
{
	public class FormDisplayModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public FormStatus Status { get; set; } = FormStatus.Public;
		public string? OwnerId { get; set; }
		public List<QuestionDisplayModel>? Questions { get; set; }

		public FormDisplayModel(Form form)
		{
			Id = form.Id;
			Title = form.Title;
			Status = form.Status;
			OwnerId = form.OwnerId;
			Questions = form.Questions?.Select(s => new QuestionDisplayModel(s)).ToList();
		}
	}
}
