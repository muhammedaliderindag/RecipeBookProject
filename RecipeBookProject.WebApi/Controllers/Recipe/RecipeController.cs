using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Data.Entities;
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
            if (User?.Identity?.IsAuthenticated != true)
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Oturum doğrulanamadı.", 401));

            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var userId))
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Kullanıcı kimliği bulunamadı.", 401));

            var response = await _recipeService.GetProductCommentsAsync(id, userId, page, pageSize, sort, ct);

            return Ok(response);
        }


        [HttpPost("{id:int}/comments")]
        public async Task<ActionResult<GeneralResponse<PagedResult<CommentDto>>>> PostComments(int id,[FromBody] AddCommentDto dto,CancellationToken ct = default)
        {
            if (User?.Identity?.IsAuthenticated != true)
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Oturum doğrulanamadı.", 401));

            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var userId))
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Kullanıcı kimliği bulunamadı.", 401));

            var response = await _recipeService.AddCommentsAsync(id, userId, dto, ct);

            return Ok(response);
        }

        [HttpPost("save-recipe")]
        public async Task<ActionResult<GeneralResponse<NoData>>> SaveRecipe([FromBody] SaveRecipeRequestDto request,CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.SaveRecipeAsync(userId, request.ProductId, request.IsSaved, ct);
            return Ok(result);
        }
        [HttpGet("getsavedrecipe")]
        public async Task<ActionResult<GeneralResponse<bool>>> GetSaveRecipe([FromQuery] int recipeId, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<bool>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.GetSavedRecipeAsync(userId,recipeId, ct);
            return Ok(result);
        }
        [HttpPost("vote-recipe")]
        public async Task<ActionResult<GeneralResponse<NoData>>> VoteRecipe([FromBody] VoteRecipeDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.VoteRecipeAsync(userId, request.productid, request.vote, ct);
            return Ok(result);
        }

        [HttpGet("getvotedrecipe")]
        public async Task<ActionResult<GeneralResponse<VoteRecipeDto>>> GetVotedRecipe([FromQuery] int recipeId, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<VoteRecipeDto>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.GetVotedRecipeAsync(userId, recipeId, ct);
            return Ok(result);
        }

        [HttpGet("getabusecategory")]
        public async Task<ActionResult<GeneralResponse<VoteRecipeDto>>> GetAbuseCategory(CancellationToken ct)
        {
            var result = await _recipeService.GetAbuseCategory();
            return Ok(result);
        }

        [HttpPost("saveabuse")]
        public async Task<ActionResult<GeneralResponse<NoData>>> SaveAbuse([FromBody] AbuseRequestDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));
            var result = await _recipeService.SaveAbuseAsync(userId, request);
            return Ok(result);
        }
        [HttpPost("createrecipe")]
        public async Task<ActionResult<GeneralResponse<NoData>>> CreateRecipe([FromBody] CreateProductDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));
            var result = await _recipeService.CreateRecipeAsync(userId, request);
            return Ok(result);
        }
    }
}
