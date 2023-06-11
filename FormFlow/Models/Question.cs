using System.ComponentModel.DataAnnotations;
using FormFlow.Models.Enums;

namespace FormFlow.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "FormId is required.")]
        public string FormId { get; set; }
        [Required(ErrorMessage = "Text is required.")]
        public string Text { get; set; }
        [Required(ErrorMessage = "Type is required.")]
        public QuestionType Type { get; set; }
        public List<Option>? Options { get; set; }
    }
}
