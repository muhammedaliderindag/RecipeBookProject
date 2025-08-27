using Azure;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Contracts;
using RecipeBookProject.Contracts;
using RecipeBookProject.Contracts.Recipes;
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
        Task<GeneralResponse<NoData>> SaveRecipeAsync(int userId, int productId, bool isSaved, CancellationToken ct);
        Task<GeneralResponse<NoData>> AddCommentsAsync(int productid, int id, AddCommentDto dto, CancellationToken ct);
        Task<GeneralResponse<NoData>> VoteRecipeAsync(int userId, int productId, int vote, CancellationToken ct = default);
        Task<GeneralResponse<bool>> GetSavedRecipeAsync(int userId, int recipeId, CancellationToken ct = default);
        Task<GeneralResponse<VoteRecipeDto>> GetVotedRecipeAsync(int userId, int recipeId, CancellationToken ct = default);
        Task<GeneralResponse<List<AbuseCategoryDto>>> GetAbuseCategory();
        Task<GeneralResponse<NoData>> SaveAbuseAsync(int userId, AbuseRequestDto request);
        Task<GeneralResponse<NoData>> CreateRecipeAsync(int userId, CreateProductDto request);
        Task<ProductWithIngredientsDto?> GetRecipeWithIngredientsAsync(int productId, CancellationToken ct = default);

    }
}