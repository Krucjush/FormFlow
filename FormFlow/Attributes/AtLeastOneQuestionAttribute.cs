using FormFlow.Models;
using System.ComponentModel.DataAnnotations;

namespace FormFlow.Attributes
{
    public class AtLeastOneQuestionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var questions = value as List<Question>;

            if (questions == null || questions.Count == 0)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
