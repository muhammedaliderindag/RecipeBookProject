using RecipeBookProject.Data.Entities;

namespace RecipeBookProject.DataAccess.Repositories.Abstract
{
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        Task<List<Ingredient>> GetActiveIngredientsAsync(CancellationToken ct = default);
        Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Ingredient> AddAsync(Ingredient entity, CancellationToken ct = default);
        Task<Ingredient> UpdateAsync(Ingredient entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}

