using System.ComponentModel.DataAnnotations;
using FormFlow.Models.Enums;

namespace FormFlow.Models
{
    public class Form
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Questions are required.")]
        [MinLength(1, ErrorMessage = "At least one question is required.")]
        public List<Question> Questions { get; set; }
        public FormStatus Status { get; set; } = FormStatus.Public;
        public string? OwnerId { get; set; }
    }
}
