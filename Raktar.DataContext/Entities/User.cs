using System;
using System.Collections.Generic;

namespace Raktar.DataContext;

public partial class User
{
    public int UserId { get; set; }

    public int? TelephoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public byte[]? Password { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Privilage> Privilages { get; set; } = new List<Privilage>();
}
