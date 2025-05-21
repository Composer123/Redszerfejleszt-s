using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.Entities;

public partial class User
{
    public int UserId { get; set; }

    [Phone]
    [MaxLength(16)]
    public string TelephoneNumber { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public byte[] Password { get; set; }

    public ICollection<Order> Orders { get; set; } = [];

    public ICollection<Order> CarriedOrders { get; set; } = [];

    public ICollection<Role> Roles { get; set; } = [];
}
