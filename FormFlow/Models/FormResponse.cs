using System.ComponentModel.DataAnnotations;

namespace FormFlow.Models
{
	public class FormResponse
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "FormId is required.")]
		public int FormId { get; set; }
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }
		public List<ResponseEntry>? Responses { get; set; }
	}
}
