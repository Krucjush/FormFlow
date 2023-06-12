using System.ComponentModel.DataAnnotations;

namespace FormFlow.ViewModels
{
    public class OptionViewModel
    {
        [Required(ErrorMessage = "Option text is required.")]
        public string Text { get; set; }
    }
}
