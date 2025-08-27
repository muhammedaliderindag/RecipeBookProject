using RecipeBookProject.Contracts;
using RecipeBookProject.Contracts.Recipes;

namespace RecipeBookProject.Business.Abstract
{
    public interface IIngredientService
    {
        Task<List<IngredientDto>> GetAllAsync(CancellationToken ct = default);
        Task<IngredientDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CreateAsync(IngredientDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, IngredientDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}

