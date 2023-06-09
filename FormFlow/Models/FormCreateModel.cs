using FormFlow.Models.Enums;

namespace FormFlow.Models
{
    public class FormCreateModel
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public FormStatus? Status { get; set; }
    }
}
