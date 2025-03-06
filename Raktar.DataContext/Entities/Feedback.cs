using System;
using System.Collections.Generic;

namespace Raktar.DataContext;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? FeedbackText { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
