using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

[Index("IngredientId", Name = "IX_RecipeIngredients_IngredientId")]
[Index("PendingProductId", Name = "IX_RecipeIngredients_PendingProductId")]
public partial class RecipeIngredient
{
    [Key]
    public int RecipeIngredientId { get; set; }

    public int? ProductId { get; set; }

    public int PendingProductId { get; set; }

    public int IngredientId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Quantity { get; set; }

    [StringLength(50)]
    public string Unit { get; set; } = null!;

    [StringLength(200)]
    public string? Notes { get; set; }

    public int ServingSize { get; set; }

    [ForeignKey("IngredientId")]
    [InverseProperty("RecipeIngredients")]
    public virtual Ingredient Ingredient { get; set; } = null!;

    [ForeignKey("PendingProductId")]
    [InverseProperty("RecipeIngredients")]
    public virtual PendingProduct PendingProduct { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("RecipeIngredients")]
    public virtual Product? Product { get; set; }
}
