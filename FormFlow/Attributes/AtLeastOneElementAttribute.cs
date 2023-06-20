using System.Collections;
using System.ComponentModel.DataAnnotations;
using FormFlow.Models;

namespace FormFlow.Attributes
{
	public class AtLeastOneElementAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext())
			{
				return new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
		}
	}
}
