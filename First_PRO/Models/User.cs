using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace First_PRO.Models;

public partial class User
{
    public decimal Id { get; set; }

    public string? Username { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public decimal? Gender { get; set; }

    public decimal? RoleId { get; set; }

    public string? Address { get; set; }

    public DateTime? Date { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
    [NotMapped]
    public IFormFile ImageFile { get; set; }
}
