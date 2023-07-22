using FormFlow.Models.Enums;

namespace FormFlow.Models.ViewModels
{
	public class QuestionViewModel
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public QuestionType Type { get; set; }
		public List<OptionViewModel>? Options { get; set; }
	}
}
