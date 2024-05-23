using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace First_PRO.Models;

public partial class Category
{
    [DisplayName("Categories")]
    public decimal Id { get; set; }
    
    public string? Name { get; set; }
    [DisplayName("Image Path")]
    public string? ImagePath { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }


    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
