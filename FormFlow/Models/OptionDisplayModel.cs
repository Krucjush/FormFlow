namespace FormFlow.Models
{
	public class OptionDisplayModel
	{
		public int Id { get; set; }
		public string Text { get; set; }

		public OptionDisplayModel(Option option)
		{
			Id = option.Id;
			Text = option.Text;
		}
	}
}
