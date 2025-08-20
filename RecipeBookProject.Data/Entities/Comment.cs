using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

public partial class Comment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public bool Secret { get; set; }

    [StringLength(150)]
    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Comments")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Comments")]
    public virtual User User { get; set; } = null!;
}
