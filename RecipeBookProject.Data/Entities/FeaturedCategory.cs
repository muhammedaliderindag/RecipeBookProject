using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

[Table("FeaturedCategory")]
public partial class FeaturedCategory
{
    [Key]
    public int FeaturedCategoryId { get; set; }

    [StringLength(50)]
    public string FeaturedCategoryName { get; set; } = null!;

    [InverseProperty("FeaturedCategory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
