using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(50)]
    public string ProductName { get; set; } = null!;

    [StringLength(50)]
    public string ProductShortDesc { get; set; } = null!;

    public string? ProductDetailedText { get; set; }

    public int CategoryId { get; set; }

    public int? FeaturedCategoryId { get; set; }

    [StringLength(50)]
    public string? ImageUrl { get; set; }

    public int? ProductionTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [ForeignKey("FeaturedCategoryId")]
    [InverseProperty("Products")]
    public virtual FeaturedCategory? FeaturedCategory { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ProductAbuse> ProductAbuses { get; set; } = new List<ProductAbuse>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductVote> ProductVotes { get; set; } = new List<ProductVote>();

    [InverseProperty("Product")]
    public virtual ICollection<SavedProduct> SavedProducts { get; set; } = new List<SavedProduct>();
}
