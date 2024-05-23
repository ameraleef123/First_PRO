using System;
using System.Collections.Generic;

namespace First_PRO.Models;

public partial class Testimonial
{
    public decimal Id { get; set; }

    public decimal? RecipeId { get; set; }

    public decimal? UserId { get; set; }

    public DateTime? Date { get; set; }

    public decimal? Rate { get; set; }

    public string? Description { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User? User { get; set; }
}
