using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Data.Context;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;

namespace RecipeBookProject.DataAccess.Repositories.Concrete
{
    public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
    {
        private readonly RecipeBookProjectDbContext _ctx;
        public IngredientRepository(RecipeBookProjectDbContext ctx) : base(ctx) => _ctx = ctx;

        public async Task<List<Ingredient>> GetActiveIngredientsAsync(CancellationToken ct = default)
        {
            return await _set.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(ct);
        }

        public async Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _set.FirstOrDefaultAsync(x => x.IngredientId == id, ct);
        }

        public async Task<Ingredient> AddAsync(Ingredient entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
            return entity;
        }

        public async Task<Ingredient> UpdateAsync(Ingredient entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetByIdAsync(id, ct);
            if (entity == null) return false;
            
            _set.Remove(entity);
            return true;
        }
    }
}

