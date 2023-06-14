using FormFlow.Attributes;
using FormFlow.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FormFlow.Models
{
	public class Form
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Title is required.")]
		public string Title { get; set; }
		public List<Question>? Questions { get; set; }
		public FormStatus Status { get; set; } = FormStatus.Public;
		public string? OwnerId { get; set; }
	}
}
