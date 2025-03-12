using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Cart
{
    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public int? FeedbackId { get; set; }

    public Feedback? Feedback { get; set; }

    public Order Order { get; set; }
}
