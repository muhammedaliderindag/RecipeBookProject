using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Data.Entities;

namespace RecipeBookProject.Data.Context;

public partial class RecipeBookProjectDbContext : DbContext
{
    public RecipeBookProjectDbContext(DbContextOptions<RecipeBookProjectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<FeaturedCategory> FeaturedCategories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductVote> ProductVotes { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<SavedProduct> SavedProducts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(d => d.Product).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Products");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.FeaturedCategory).WithMany(p => p.Products).HasConstraintName("FK_Products_FeaturedCategory");
        });

        modelBuilder.Entity<ProductVote>(entity =>
        {
            entity.HasOne(d => d.Product).WithMany(p => p.ProductVotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductVotes_Products");

            entity.HasOne(d => d.User).WithMany(p => p.ProductVotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductVotes_Users");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC071DB4A61F");

            entity.Property(e => e.Created).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_Users");
        });

        modelBuilder.Entity<SavedProduct>(entity =>
        {
            entity.HasOne(d => d.Product).WithMany(p => p.SavedProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedProducts_Products");

            entity.HasOne(d => d.User).WithMany(p => p.SavedProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedProducts_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
