using System;
using System.Collections.Generic;

namespace Raktar;

public partial class Privilage
{
    public int RoleId { get; set; }

    public int? UserId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User? User { get; set; }
}
