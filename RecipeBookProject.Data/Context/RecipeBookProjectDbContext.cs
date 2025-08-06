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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
