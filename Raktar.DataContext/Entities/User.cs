﻿using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class User
{
    public int UserId { get; set; }

    public int TelephoneNumber { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public byte[] Password { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
