using Azure;
using RecipeBookProject.Business.Models; 
using RecipeBookProject.Data.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Abstract
{
    public interface IRecipeService
    {
		Task<GeneralResponse<List<ProductDto>>> GetShortRecipesAsync();
        Task<GeneralResponse<ProductDto>> GetDetailedRecipeAsync(int productId);
        Task<GeneralResponse<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<GeneralResponse<List<ProductDto>>> GetSearchedProductsAsync(string query,int? categoryid);
        Task<GeneralResponse<PagedResult<CommentDto>>> GetProductCommentsAsync(int id, int userId, int page, int pageSize, string sort, CancellationToken ct);

    }
}