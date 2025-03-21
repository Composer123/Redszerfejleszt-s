﻿using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
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
