using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.Dtos
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }

        public string FeedbackText { get; set; }
    }
    public class FeedbackCreateDTO
    {
        [Required]
        public string FeedbackText { get; set; }
    }
}
