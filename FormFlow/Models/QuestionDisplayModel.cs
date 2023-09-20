using FormFlow.Models.Enums;

namespace FormFlow.Models
{
	public class QuestionDisplayModel
	{
		public int Id { get; set; }
		public int FormId { get; set; }
		public string Text { get; set; }
		public QuestionType? Type { get; set; }
		public List<OptionDisplayModel>? Options { get; set; }
		public bool Required { get; set; } = true;
		public bool MultipleChoice { get; set; }

		public QuestionDisplayModel(Question question)
		{
			Id = question.Id;
			FormId = question.FormId;
			Text = question.Text;
			Type = question.Type;
			Options = question.Options?.Select(s => new OptionDisplayModel(s)).ToList();
			Required = question.Required;
			MultipleChoice = question.MultipleChoice;
		}
	}
}
