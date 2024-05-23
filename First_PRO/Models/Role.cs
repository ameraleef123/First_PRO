﻿using System;
using System.Collections.Generic;

namespace First_PRO.Models;

public partial class Role
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
