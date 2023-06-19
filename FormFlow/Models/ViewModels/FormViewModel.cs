using FormFlow.Models.Enums;

namespace FormFlow.Models.ViewModels
{
	public class FormViewModel
	{
		public IEnumerable<Form> ListForms { get; set; }
		public Form Form { get; set; }
        public Question Question { get; set; }
        public List<Question> Questions { get; set; }
        public FormStatus Status { get; set; } = FormStatus.Public;
        public List<QuestionType> QuestionTypes { get; set; }
	}
}