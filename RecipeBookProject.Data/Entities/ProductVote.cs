using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

public partial class ProductVote
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int Vote { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductVotes")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ProductVotes")]
    public virtual User User { get; set; } = null!;
}
