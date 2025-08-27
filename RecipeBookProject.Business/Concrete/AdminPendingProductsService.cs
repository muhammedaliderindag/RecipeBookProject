using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Recipes;
using RecipeBookProject.Data.Context;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Concrete;

public class AdminPendingProductsService : IAdminPendingProductsService
{
    private readonly IPendingProductRepository _repo;
    private readonly IProductRepository _productRepo;
    private readonly RecipeBookProject.Data.Context.RecipeBookProjectDbContext _db;
    public AdminPendingProductsService(IPendingProductRepository repo,IProductRepository productRepo, RecipeBookProject.Data.Context.RecipeBookProjectDbContext db)
    {
        _repo = repo;
        _productRepo = productRepo;
        _db = db;
    }

    public async Task<PagedResult<AdminPendingProductDto>> GetAsync(PendingProductsQuery input, CancellationToken ct = default)
    {
        var page = input.Page < 1 ? 1 : input.Page;
        var pageSize = input.PageSize is < 1 or > 100 ? 10 : input.PageSize;

        // TIPI ACIK BELIRT
        IQueryable<PendingProduct> q = _repo.Query().AsNoTracking();

        // Filtreler
        if (!string.Equals(input.Status, "all", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(input.Status, "pending", StringComparison.OrdinalIgnoreCase))
                q = q.Where(x => !x.IsApproved);
            else if (string.Equals(input.Status, "approved", StringComparison.OrdinalIgnoreCase))
                q = q.Where(x => x.IsApproved);
        }

        if (input.CategoryId is > 0)
            q = q.Where(x => x.CategoryId == input.CategoryId);

        if (!string.IsNullOrWhiteSpace(input.Query))
        {
            var query = input.Query.Trim();
            q = q.Where(x => x.ProductName.Contains(query) || x.ProductShortDesc.Contains(query));
        }

        var total = await q.CountAsync(ct);

        // Sıralama + sayfalama + MANUEL PROJECTION
        // Aynı filtreleri burada da uygula (sabit !x.IsApproved filtresini kaldır)
        IQueryable<PendingProduct> resultQuery = _repo.Query()
            .Include(x => x.Category)
            .Include(x => x.User)
            .Include(x => x.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient);

        // Status filtrelerini tekrar uygula
        if (!string.Equals(input.Status, "all", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(input.Status, "pending", StringComparison.OrdinalIgnoreCase))
                resultQuery = resultQuery.Where(x => !x.IsApproved);
            else if (string.Equals(input.Status, "approved", StringComparison.OrdinalIgnoreCase))
                resultQuery = resultQuery.Where(x => x.IsApproved);
        }

        // Diğer filtreleri de uygula
        if (input.CategoryId is > 0)
            resultQuery = resultQuery.Where(x => x.CategoryId == input.CategoryId);

        if (!string.IsNullOrWhiteSpace(input.Query))
        {
            var query = input.Query.Trim();
            resultQuery = resultQuery.Where(x => x.ProductName.Contains(query) || x.ProductShortDesc.Contains(query));
        }

        var result = await resultQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((input.Page - 1) * input.PageSize)
            .Take(input.PageSize)
            .Select(x => new AdminPendingProductDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductShortDesc = x.ProductShortDesc,
                ProductDetailedText = x.ProductDetailedText,
                CategoryId = x.CategoryId,
                ImageUrl = x.ImageUrl,
                ProductionTime = x.ProductionTime,
                CreatedAt = x.CreatedAt,
                CategoryName = x.Category.CategoryName,
                UserName = $"{x.User.FirstName} {x.User.LastName}",
                BaseServingSize = x.BaseServingSize, // PendingProduct'tan al
                IsApproved = x.IsApproved, // Onay durumu
                ApprovedAt = x.ApprovedAt, // Onaylanma tarihi
                Ingredients = x.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit,
                    Notes = ri.Notes
                }).ToList()
            })
            .ToListAsync(ct);

        return new PagedResult<AdminPendingProductDto>(result, total, page, pageSize);
    }


    public async Task<bool> ApproveAsync(int productId, CancellationToken ct = default)
    {
        // Pending kaydı getir
        var entity = await _repo.GetByIdAsync(productId, ct);
        if (entity is null) return false;

        // Zaten onaylıysa tekrar ekleme yapma (idempotent)
        if (!entity.IsApproved)
        {
            // Debug: ImageUrl kopyalama kontrolü
            Console.WriteLine($"DEBUG: ApproveAsync - PendingProduct ImageUrl: {entity.ImageUrl}");
            Console.WriteLine($"DEBUG: ApproveAsync - PendingProduct ImageUrl length: {entity.ImageUrl?.Length}");
            
            // --- Manuel mapping: PendingProduct -> Product
            var newProduct = new Product
            {
                // ProductId: identity, set etme!
                ProductName = entity.ProductName,
                ProductShortDesc = entity.ProductShortDesc,
                ProductDetailedText = entity.ProductDetailedText,
                CategoryId = entity.CategoryId,
                FeaturedCategoryId = null,                  // İstersen business kuralınla set edebilirsin
                ImageUrl = entity.ImageUrl, // Product.ImageUrl field'ı sınırsız, kısaltmaya gerek yok
                ProductionTime = entity.ProductionTime, // int -> int?
                BaseServingSize = entity.BaseServingSize, // BaseServingSize'ı da kopyala
                CreatedAt = DateTime.UtcNow
            };
            
            Console.WriteLine($"DEBUG: ApproveAsync - New Product ImageUrl: {newProduct.ImageUrl}");
            Console.WriteLine($"DEBUG: ApproveAsync - New Product ImageUrl length: {newProduct.ImageUrl?.Length}");

            // Önce newProduct'ı kaydet ve ProductId'yi al
            await _productRepo.AddAsync(newProduct, ct);
            await _productRepo.SaveChangesAsync(ct); // ProductId'yi almak için önce kaydet

            // RecipeIngredients'lara ProductId'yi ekle (PendingProductId'yi silme)
            var recipeIngredients = await _db.RecipeIngredients
                .Where(ri => ri.PendingProductId == productId)
                .ToListAsync(ct);

            foreach (var ingredient in recipeIngredients)
            {
                ingredient.ProductId = newProduct.ProductId; // Şimdi ProductId dolu olacak
            }

            // Pending'i onayla
            entity.IsApproved = true;
            entity.ApprovedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(entity, ct);

            // Products tablosunda görünürlüğü true yap
            newProduct.IsVisible = true;
            _db.Products.Update(newProduct);

            // Aynı DbContext scope'unda olduğumuz için tek SaveChanges tüm değişiklikleri yazar
            await _productRepo.SaveChangesAsync(ct);
        }

        return true;
    }


    public async Task<bool> RejectAsync(int productId, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(productId, ct);
        if (entity is null) return false;

        await _repo.RemoveAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> UpdateProductAsync(int productId, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _repo.GetByIdAsync(productId, ct);
        if (product == null) return false;

        // Ürün bilgilerini güncelle
        product.ProductName = dto.ProductName;
        product.ProductShortDesc = dto.ProductShortDesc;
        product.CategoryId = dto.CategoryId;
        product.ImageUrl = dto.ImageUrl;
        product.ProductionTime = (int)dto.ProductionTime;
        product.ProductDetailedText = dto.ProductDetailedText;
        product.BaseServingSize = dto.BaseServingSize;

        // Mevcut malzemeleri sil (hem ProductId hem PendingProductId ile)
        var existingIngredients = await _db.RecipeIngredients
            .Where(ri => ri.PendingProductId == productId || ri.ProductId == productId)
            .ToListAsync(ct);
        
        _db.RecipeIngredients.RemoveRange(existingIngredients);

        // Yeni malzemeleri ekle
        foreach (var ingredientDto in dto.Ingredients.Where(i => i.IngredientId > 0))
        {
            var recipeIngredient = new RecipeIngredient
            {
                PendingProductId = productId, // PendingProduct olarak kalacak
                ProductId = null, // ProductId NULL olacak
                IngredientId = ingredientDto.IngredientId,
                Quantity = ingredientDto.Quantity,
                Unit = ingredientDto.Unit,
                Notes = ingredientDto.Notes,
                ServingSize = dto.BaseServingSize
            };
            _db.RecipeIngredients.Add(recipeIngredient);
        }

        // PendingProduct'ı güncelle
        await _repo.UpdateAsync(product, ct);
        
        // Tüm değişiklikleri kaydet
        var allChangesSaved = await _db.SaveChangesAsync(ct) > 0;
        
        Console.WriteLine($"DEBUG: UpdateProductAsync - Product updated: {allChangesSaved}");
        Console.WriteLine($"DEBUG: UpdateProductAsync - Ingredients count: {dto.Ingredients?.Count ?? 0}");
        
        return allChangesSaved;
    }

    public async Task<bool> ToggleProductVisibilityAsync(int productId, CancellationToken ct = default)
    {
        var product = await _repo.GetByIdAsync(productId, ct);
        if (product == null) return false;

        // Görünürlüğü değiştir (onaylı <-> onay bekliyor)
        product.IsApproved = !product.IsApproved;
        
        if (product.IsApproved)
        {
            product.ApprovedAt = DateTime.UtcNow;
        }
        else
        {
            product.ApprovedAt = null;
        }

        // Eğer onaylanmışsa Products tablosunda da görünürlüğü güncelle
        if (product.IsApproved)
        {
            // PendingProducts tablosundaki ProductId ile Products tablosunda arama yap
            var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
            Console.WriteLine($"DEBUG: ToggleVisibility - Onaylandı, ProductId: {productId}, Found Product: {existingProduct != null}");
            if (existingProduct != null)
            {
                existingProduct.IsVisible = true;
                _db.Products.Update(existingProduct);
                Console.WriteLine($"DEBUG: ToggleVisibility - Product.IsVisible set to true for ProductId: {productId}");
            }
        }
        else
        {
            // Onay kaldırıldıysa Products tablosunda gizle
            var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
            Console.WriteLine($"DEBUG: ToggleVisibility - Onay kaldırıldı, ProductId: {productId}, Found Product: {existingProduct != null}");
            if (existingProduct != null)
            {
                Console.WriteLine($"DEBUG: ToggleVisibility - Before: Product.IsVisible = {existingProduct.IsVisible}");
                existingProduct.IsVisible = false;
                _db.Products.Update(existingProduct);
                Console.WriteLine($"DEBUG: ToggleVisibility - After: Product.IsVisible = {existingProduct.IsVisible}");
                Console.WriteLine($"DEBUG: ToggleVisibility - Product.IsVisible set to false for ProductId: {productId}");
            }
            else
            {
                Console.WriteLine($"DEBUG: ToggleVisibility - Product not found in Products table for ProductId: {productId}");
            }
        }

        await _repo.UpdateAsync(product, ct);
        
        // Hem PendingProducts hem de Products tablosundaki değişiklikleri kaydet
        var saveResult = await _db.SaveChangesAsync(ct);
        Console.WriteLine($"DEBUG: ToggleVisibility - SaveChangesAsync result: {saveResult}");
        
        // SaveChangesAsync sonrasında Products tablosundaki değeri tekrar kontrol et
        if (saveResult > 0)
        {
            var verifyProduct = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
            if (verifyProduct != null)
            {
                Console.WriteLine($"DEBUG: ToggleVisibility - Verification: Product.IsVisible = {verifyProduct.IsVisible}");
            }
        }
        
        return saveResult > 0;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(int days, CancellationToken ct = default)
    {
        var totalProducts = await _db.Products.CountAsync(ct);
        var pendingCount = await _db.PendingProducts.CountAsync(ct);
        var totalComments = await _db.Comments.CountAsync(ct);
        var totalReports = await _db.ProductAbuses.CountAsync(ct);

        var since = DateTime.UtcNow.Date.AddDays(-89);
        var weekly = await _db.Products
            .Where(p => p.CreatedAt != null && p.CreatedAt >= since)
            .GroupBy(p => p.CreatedAt!.Value.Date)
            .Select(g => new WeeklyPointDto { Date = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var cat = await _db.Products
            .GroupBy(p => p.Category.CategoryName)
            .Select(g => new { CategoryName = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);
        var sum = cat.Sum(x => x.Count);
        var catShare = cat.Select(x => new CategoryShareDto
        {
            CategoryName = x.CategoryName,
            Count = x.Count,
            Percent = sum == 0 ? 0 : (double)x.Count * 100.0 / sum
        }).ToList();

        var reported = await _db.ProductAbuses
            .OrderByDescending(a => a.CreatedAt)
            .Take(100)
            .Select(a => new ReportedItemDto
            {
                Title = a.Product.ProductName,
                Reason = a.Category.CategoryName,
                Count = 1
            })
            .ToListAsync(ct);

        var comments = await _db.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Take(100)
            .Select(c => new CommentItemDto
            {
                Author = c.User.FirstName + " " + c.User.LastName,
                Text = c.Text,
                Date = c.CreatedAt,
                Avatar = c.User.ProfileImageUrl
            })
            .ToListAsync(ct);

        return new AdminDashboardDto
        {
            TotalProducts = totalProducts,
            PendingCount = pendingCount,
            TotalComments = totalComments,
            TotalReports = totalReports,
            WeeklyPublished = weekly.OrderBy(x => x.Date).ToList(),
            CategoryDistribution = catShare,
            ReportedTop = reported,
            RecentComments = comments
        };
    }
}