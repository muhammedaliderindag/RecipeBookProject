using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeBookProject.Data.Entities;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    public string PasswordHashed { get; set; } = null!;

    [StringLength(50)]
    public string? Role { get; set; }

    public string? ProfileImageUrl { get; set; }

    [StringLength(50)]
    public string? Bio { get; set; }

    [StringLength(50)]
    public string? PersonalWebsite { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("User")]
    public virtual ICollection<ProductVote> ProductVotes { get; set; } = new List<ProductVote>();

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("User")]
    public virtual ICollection<SavedProduct> SavedProducts { get; set; } = new List<SavedProduct>();
}
