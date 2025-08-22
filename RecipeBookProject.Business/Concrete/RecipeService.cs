
using Azure;
using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Globalization;
using System.Runtime.Intrinsics.X86;
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

        var response = string.IsNullOrWhiteSpace(query)
            ? await _recipeRepository.GetSearchedProductsRepositoryAsync(category)
            : await _recipeRepository.GetSearchedProductsRepository(query, category);

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

    static string FirstInitial(string? s) // İlk haftayı büyük yapmak için Türkçe kültürünü kullanıyoruz
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        return s.Trim().Substring(0, 1).ToUpper(new CultureInfo("tr-TR"));
    }

    public async Task<GeneralResponse<NoData>> SaveRecipeAsync(int userId, int productId, bool isSaved, CancellationToken ct)
    {
        var repositoryResponse = await _recipeRepository.SaveRecipeRepositoryAsync(userId, productId, isSaved);
        if (!repositoryResponse)
        {
            throw new Exception("Veriler kaydedilemedi");
        }
        return GeneralResponse<NoData>.Success("Veriler başarıyla kaydedildi", 200);
    }

    public async Task<GeneralResponse<bool>> GetSavedRecipeAsync(int userId, int productId, CancellationToken ct)
    {
        var repositoryResponse = await _recipeRepository.GetSavedRecipeRepositoryAsync(userId, productId);
        return GeneralResponse<bool>.Success(repositoryResponse, "Başarıyla saved çekildi.",200);
    }

    public async Task<GeneralResponse<NoData>> VoteRecipeAsync(int userId, int productId, int vote, CancellationToken ct)
    {
        var repositoryResponse = await _recipeRepository.VoteRecipeRepositoryAsync(userId, productId, vote);
        if (!repositoryResponse)
        {
            throw new Exception("Veriler kaydedilemedi");
        }
        return GeneralResponse<NoData>.Success("Veriler başarıyla kaydedildi", 200);
    }

    public async Task<GeneralResponse<VoteRecipeDto>> GetVotedRecipeAsync(int userId, int productId, CancellationToken ct)
    {
        var (totalVoters, avg, userVote) = await _recipeRepository.GetVotedRecipeRepositoryAsync(userId, productId);

        var dtoMapping = new VoteRecipeDto
        {
            productid = productId,
            vote = userVote ?? 0,                  // kullanıcı hiç oy vermemişse 0
            avgVote = avg,        // double → int (yuvarlanmış)
            totalVoters = totalVoters
        };
        return GeneralResponse<VoteRecipeDto>.Success(dtoMapping, "Başarıyla saved çekildi.", 200);
    }
}