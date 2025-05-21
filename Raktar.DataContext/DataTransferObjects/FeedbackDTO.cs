using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public string? FeedbackText { get; set; }
        public byte StarRating { get; set; }
        // New: an embedded order object containing some order details.
        public OrderDTO? Order { get; set; }
    }


    public class FeedbackCreateDTO
    {
        [Required]
        [Range(0,5)]
        public byte StarRating { get; set; }
        public string? FeedbackText { get; set; }
        public int OrderId { get; set; }
    }
}
