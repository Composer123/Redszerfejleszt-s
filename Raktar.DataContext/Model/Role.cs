using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Model;

public partial class Role
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual Privilage? Privilage { get; set; }
}
