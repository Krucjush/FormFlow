using FormFlow.Models.Enums;

namespace FormFlow.Models
{
    public class FormUpdateModel
    {
        public string? Title { get; set; }
        public List<Question>? Questions { get; set; }
        public FormStatus? Status { get; set; }
    }
}
