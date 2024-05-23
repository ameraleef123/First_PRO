using System;
using System.Collections.Generic;

namespace First_PRO.Models;

public partial class Guest
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public DateTime? Date { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Question { get; set; }
}
