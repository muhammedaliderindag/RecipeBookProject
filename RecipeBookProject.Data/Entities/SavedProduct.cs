using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

public partial class SavedProduct
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("SavedProducts")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SavedProducts")]
    public virtual User User { get; set; } = null!;
}
