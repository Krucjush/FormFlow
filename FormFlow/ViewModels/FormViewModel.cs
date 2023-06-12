using System.ComponentModel.DataAnnotations;
using FormFlow.Attributes;
using FormFlow.Models.Enums;

namespace FormFlow.ViewModels
{
    public class FormViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [AtLeastOneQuestion(ErrorMessage = "At least one question is required.")]
        public List<QuestionViewModel> Questions { get; set; }
        public FormStatus Status { get; set; }
    }
}
