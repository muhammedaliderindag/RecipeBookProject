
using Azure;
using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Globalization;
using System.Security.Claims;
using static RecipeBookProject.Business.Middleware.ExceptionHandlerMiddleware;
public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<GeneralResponse<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categoryList = await _recipeRepository.GetAllCategoriesRepository();
        if (categoryList == null)
            throw new Exception("Veriler çekilemedi");

        var ResponseList = categoryList.Select(p => new CategoryDto
        {
            CategoryId = p.CategoryId,
            CategoryName = p.CategoryName,
        }).ToList();

        return GeneralResponse<List<CategoryDto>>.Success(ResponseList, "Veriler başarıyla çekildi", 200);
    }

    public async Task<GeneralResponse<ProductDto>> GetDetailedRecipeAsync(int productId)
    {
        var response = await _recipeRepository.GetDetailedRecipeRepository(productId);
        if (response == null)
            throw new Exception("Veriler çekilemedi");

        var dto = new ProductDto
        {
            ProductId = response.ProductId,
            ProductName = response.ProductName,
            ProductShortDesc = response.ProductShortDesc,
            ProductDetailedText = response.ProductDetailedText,
            CategoryId = response.CategoryId,
            ImageUrl = response.ImageUrl,
            ProductionTime = response.ProductionTime,
            FeaturedCategory = response.FeaturedCategory == null ? null : new FeaturedCategoryDto
            {
                FeaturedCategoryId = response.FeaturedCategory.FeaturedCategoryId,
                FeaturedCategoryName = response.FeaturedCategory.FeaturedCategoryName
            },
            Category = response.Category == null ? null : new CategoryDto
            {
                CategoryId = response.Category.CategoryId,
                CategoryName = response.Category.CategoryName
            }

        };

        return GeneralResponse<ProductDto>.Success(dto, "Veriler başarıyla çekildi", 200);
    }

    public async Task<GeneralResponse<PagedResult<CommentDto>>> GetProductCommentsAsync(
        int productId,
        int userId,                 
        int page,
        int pageSize,
        string? sort,
        CancellationToken ct)
    {
        const int MAX_PAGE_SIZE = 50;
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        if (pageSize > MAX_PAGE_SIZE) pageSize = MAX_PAGE_SIZE;

        IQueryable<Comment> q = await _recipeRepository.GetProductCommentsRepositoryAsync(productId);

        switch ((sort ?? "new").ToLowerInvariant())
        {
            case "old":
                q = q.OrderBy(c => c.CreatedAt);
                break;
            default: // "new"
                q = q.OrderByDescending(c => c.CreatedAt);
                break;
        }

        var totalCount = await q.CountAsync(ct);

        var entities = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = entities.Select(c => new CommentDto
        {
        ProductId = c.ProductId,
        UserDisplayName = c.Secret
                ? $"{FirstInitial(c.User?.FirstName)} {FirstInitial(c.User?.LastName)}".Trim()
                : $"{c.User?.FirstName} {c.User?.LastName}".Trim(),
        UserAvatarUrl = c.User?.ProfileImageUrl ?? "/images/default-avatar.png",
        IsMine = userId > 0 && c.UserId == userId,
        Text = c.Text,
        CreatedAt = c.CreatedAt,


        }).ToList();

        return GeneralResponse<PagedResult<CommentDto>>.Success(
            new PagedResult<CommentDto>(items, totalCount, page, pageSize),"Yorumlar başarıyla çekildi", 200);
    }

    public async Task<GeneralResponse<List<ProductDto>>> GetSearchedProductsAsync(string query, int? category)
    {

        if(query is null)
        {
            query = "";
        }

        var response = await _recipeRepository.GetSearchedProductsRepository(query, category);
        if (response == null)
        {
            throw new Exception("Veriler çekilemedi");
        }

        var productDtos = response.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductShortDesc = p.ProductShortDesc,
            ProductDetailedText = p.ProductDetailedText,
            CategoryId = p.CategoryId,
            ImageUrl = p.ImageUrl,
            ProductionTime = p.ProductionTime,
            FeaturedCategory = new FeaturedCategoryDto
            {
                FeaturedCategoryId = p.FeaturedCategory.FeaturedCategoryId,
                FeaturedCategoryName = p.FeaturedCategory.FeaturedCategoryName
                },
            Category = new CategoryDto
            {
                CategoryId = p.Category.CategoryId,
                CategoryName = p.Category.CategoryName
            }
            }).ToList();

        return GeneralResponse<List<ProductDto>>.Success(productDtos, "Veriler başarıyla çekildi", 200);
    }

    public async Task<GeneralResponse<List<ProductDto>>> GetShortRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllShortRecipeRepositoryAsync();
        if (recipes == null || !recipes.Any())
        {
            throw new Exception("Veriler çekilemedi");
        }
        var recipesDto = recipes.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductShortDesc = p.ProductShortDesc,
            ProductDetailedText = p.ProductDetailedText,
            CategoryId = p.CategoryId,
            ImageUrl = p.ImageUrl,
            ProductionTime = p.ProductionTime,
            FeaturedCategory = new FeaturedCategoryDto
            {
                FeaturedCategoryId = p.FeaturedCategory.FeaturedCategoryId,
                FeaturedCategoryName = p.FeaturedCategory.FeaturedCategoryName
            },
            Category = new CategoryDto
            {
                CategoryId = p.Category.CategoryId,
                CategoryName = p.Category.CategoryName
            }
        }).ToList();
        return GeneralResponse<List<ProductDto>>.Success(recipesDto, "Veriler başarıyla çekildi");
    }

    static string FirstInitial(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        // İlk harfi al ve TR kültürüne göre büyüt (İ/i doğru olsun)
        return s.Trim().Substring(0, 1).ToUpper(new CultureInfo("tr-TR"));
    }
}