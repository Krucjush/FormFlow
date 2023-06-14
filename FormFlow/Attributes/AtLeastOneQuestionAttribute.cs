using System.ComponentModel.DataAnnotations;
using FormFlow.Models;

namespace FormFlow.Attributes
{
	public class AtLeastOneQuestionAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is not List<Question> questions || questions.Count == 0)
			{
				return new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
		}
	}
}
