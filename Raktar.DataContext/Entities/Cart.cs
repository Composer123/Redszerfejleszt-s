using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Cart
{
    public int ProductId { get; set; }

    public int? OrderId { get; set; }

    public int? Quantity { get; set; }

    public int? FeedbackId { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual Order? Order { get; set; }
}
