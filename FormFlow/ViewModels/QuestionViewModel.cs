using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using FormFlow.Models.Enums;

namespace FormFlow.ViewModels
{
    public class QuestionViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Question text is required.")]
        public string Text { get; set; }
        [Required(ErrorMessage = "Question type is required.")]
        public QuestionType Type { get; set; }

        public List<string>? Answers { get; set; }

        public List<OptionViewModel> Options { get; set; }
    }
}
