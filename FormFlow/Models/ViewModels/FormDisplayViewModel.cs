namespace FormFlow.Models.ViewModels
{
	public class FormDisplayViewModel
	{
		public int FormId { get; set; }
		public string Title { get; set; }
		public List<QuestionViewModel> Questions { get; set; }
        public List<FormResponse?>? UserResponses { get; set; }
        public Form Form { get; set; }
	}
}
