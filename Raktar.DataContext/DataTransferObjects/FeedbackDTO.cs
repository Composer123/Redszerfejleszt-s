using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }

        public string? FeedbackText { get; set; }
        public byte StarRating { get; set; }
    }
    public class FeedbackCreateDTO
    {
        [Required]
        [Range(0,5)]
        public byte StarRating { get; set; }
        public string? FeedbackText { get; set; }
    }
}
