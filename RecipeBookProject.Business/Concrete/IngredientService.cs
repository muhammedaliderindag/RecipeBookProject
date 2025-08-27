using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Contracts;
using RecipeBookProject.Contracts.Recipes;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;

namespace RecipeBookProject.Business.Concrete
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repo;

        public IngredientService(IIngredientRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<IngredientDto>> GetAllAsync(CancellationToken ct = default)
        {
            var ingredients = await _repo.GetActiveIngredientsAsync(ct);
            return ingredients.Select(x => new IngredientDto
            {
                IngredientId = x.IngredientId,
                Name = x.Name,
                DefaultUnit = x.DefaultUnit,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<IngredientDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var ingredient = await _repo.GetByIdAsync(id, ct);
            if (ingredient == null) return null;

            return new IngredientDto
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                DefaultUnit = ingredient.DefaultUnit,
                IsActive = ingredient.IsActive
            };
        }

        public async Task<bool> CreateAsync(IngredientDto dto, CancellationToken ct = default)
        {
            var ingredient = new Ingredient
            {
                Name = dto.Name?.Trim(),
                DefaultUnit = string.IsNullOrWhiteSpace(dto.DefaultUnit) ? null : dto.DefaultUnit?.Trim(),
                IsActive = dto.IsActive
            };

            await _repo.AddAsync(ingredient, ct);
            return await _repo.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> UpdateAsync(int id, IngredientDto dto, CancellationToken ct = default)
        {
            var ingredient = await _repo.GetByIdAsync(id, ct);
            if (ingredient == null) return false;

            ingredient.Name = dto.Name?.Trim();
            ingredient.DefaultUnit = string.IsNullOrWhiteSpace(dto.DefaultUnit) ? null : dto.DefaultUnit?.Trim();
            ingredient.IsActive = dto.IsActive;

            await _repo.UpdateAsync(ingredient, ct);
            return await _repo.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ingredient = await _repo.GetByIdAsync(id, ct);
            if (ingredient == null) return false;

            await _repo.RemoveAsync(ingredient, ct);
            return await _repo.SaveChangesAsync(ct) > 0;
        }
    }
}

