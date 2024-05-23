using System;
using System.Collections.Generic;

namespace First_PRO.Models;

public partial class Request
{
    public decimal Id { get; set; }

    public decimal? RecipeId { get; set; }

    public decimal? UserId { get; set; }

    public DateTime? Date { get; set; }

    public string? Stuats { get; set; }

    public decimal? ByCard { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User? User { get; set; }
}
