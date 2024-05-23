using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace First_PRO.Models;

public partial class Recipe
{
    public decimal Id { get; set; }
    
    public decimal? UserId { get; set; }

    [DisplayName("Status")]
    public decimal? Stuats { get; set; }

    public decimal? CatId { get; set; }

    public DateTime? Date { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    [DisplayName("Image Path")]
    public string? ImagePath { get; set; }
    [DisplayName("Catgory")]
    public virtual Category? Cat { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual User? User { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
    [NotMapped]
    [DisplayName("Average Rating")]
    public double AverageRating { get; set; }
}
