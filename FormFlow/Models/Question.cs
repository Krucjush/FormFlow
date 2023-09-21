using FormFlow.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FormFlow.Models
{
	public class Question
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "FormId is required.")]
		public int FormId { get; set; }
		[Required(ErrorMessage = "Text is required.")]
		public string Text { get; set; }
		[Required(ErrorMessage = "Type is required.")]
		public QuestionType? Type { get; set; }
		public List<Option>? Options { get; set; }
		public bool Required { get; set; } = true;
		public bool MultipleChoice { get; set; } = false;
	}
}
