using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Data.Context;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.DataAccess.Repositories.Concrete;

public class PendingProductRepository : Repository<PendingProduct>, IPendingProductRepository
{
    public PendingProductRepository(RecipeBookProjectDbContext db) : base(db) { }

    public Task<PendingProduct?> GetByIdAsync(int id, CancellationToken ct = default)
        => _set.Include(x => x.Category).Include(x => x.User)
               .FirstOrDefaultAsync(x => x.ProductId == id, ct);
}