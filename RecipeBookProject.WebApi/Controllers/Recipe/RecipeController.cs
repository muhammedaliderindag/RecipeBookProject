using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using System.Security.Claims;

namespace RecipeBookProject.WebApi.Controllers.Recipe
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet("getShortRecipes")]
        public async Task<IActionResult> GetRecipes()
        {
            var recipes = await _recipeService.GetShortRecipesAsync();
            return Ok(recipes);
        }

        [HttpGet("GetDetailedRecipe/{productId:int}")]
        public async Task<IActionResult> GetRecipes([FromRoute] int productId)
        {
            var recipe = await _recipeService.GetDetailedRecipeAsync(productId);
            return Ok(recipe);
        }


        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var recipe = await _recipeService.GetAllCategoriesAsync();
            return Ok(recipe);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetSearchedProducts([FromQuery] string query, [FromQuery] int? category)
        {
            var response = await _recipeService.GetSearchedProductsAsync(query, category);
            return Ok(response);
        }

        [HttpGet("{id:int}/comments")]
        public async Task<ActionResult<GeneralResponse<PagedResult<CommentDto>>>> GetComments(
    int id,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sort = "new",
    CancellationToken ct = default)
        {
            //int? currentUserId = null;
            //if (User?.Identity?.IsAuthenticated == true)
            //{
            //    var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //    if (int.TryParse(idStr, out var parsed)) currentUserId = parsed;
            //}
            //if (currentUserId == null)
            //{
            //    return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Kullanıcı kimliği bulunamadı.", 401));
            //}

            int usersId = 2;

            var response = await _recipeService.GetProductCommentsAsync(id, usersId, page,pageSize,sort, ct);
            return Ok(response);
        }
    }
}
