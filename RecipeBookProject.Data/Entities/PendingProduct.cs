using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

public partial class PendingProduct
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(50)]
    public string ProductName { get; set; } = null!;

    [StringLength(50)]
    public string ProductShortDesc { get; set; } = null!;

    public string ProductDetailedText { get; set; } = null!;

    public int CategoryId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ProductionTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [Column("isApproved")]
    public bool IsApproved { get; set; }

    public int UserId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("PendingProducts")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("PendingProducts")]
    public virtual User User { get; set; } = null!;
}
