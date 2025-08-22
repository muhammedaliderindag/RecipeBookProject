using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

[Table("ProductAbuse")]
public partial class ProductAbuse
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public int UserId { get; set; }

    [StringLength(200)]
    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ProductAbuses")]
    public virtual AbuseCategory Category { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductAbuses")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ProductAbuses")]
    public virtual User User { get; set; } = null!;
}
