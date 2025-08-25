using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Common;
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
    public AdminPendingProductsService(IPendingProductRepository repo,IProductRepository productRepo)
    {
        _repo = repo;
        _productRepo = productRepo;
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
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminPendingProductDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductShortDesc = x.ProductShortDesc,
                CategoryName = x.Category.CategoryName, 
                ImageUrl = x.ImageUrl,
                ProductionTime = x.ProductionTime,
                CreatedAt = x.CreatedAt,
                ApprovedAt = x.ApprovedAt,
                IsApproved = x.IsApproved,
                Author = x.User.FirstName + " " + x.User.LastName
            })
            .ToListAsync(ct);

        return new PagedResult<AdminPendingProductDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }


    public async Task<bool> ApproveAsync(int productId, CancellationToken ct = default)
    {
        // Pending kaydı getir
        var entity = await _repo.GetByIdAsync(productId, ct);
        if (entity is null) return false;

        // Zaten onaylıysa tekrar ekleme yapma (idempotent)
        if (!entity.IsApproved)
        {
            // --- Manuel mapping: PendingProduct -> Product
            var newProduct = new Product
            {
                // ProductId: identity, set etme!
                ProductName = entity.ProductName,
                ProductShortDesc = entity.ProductShortDesc,
                ProductDetailedText = entity.ProductDetailedText,
                CategoryId = entity.CategoryId,
                FeaturedCategoryId = null,                  // İstersen business kuralınla set edebilirsin
                ImageUrl = entity.ImageUrl?.Length > 50
                                        ? entity.ImageUrl.Substring(0, 50) // Product.ImageUrl [StringLength(50)]
                                        : entity.ImageUrl,
                ProductionTime = entity.ProductionTime, // int -> int?
                CreatedAt = DateTime.UtcNow
            };

            await _productRepo.AddAsync(newProduct, ct);

            // Pending’i onayla
            entity.IsApproved = true;
            entity.ApprovedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(entity, ct);

            // Aynı DbContext scope’unda olduğumuz için tek SaveChanges tüm değişiklikleri yazar
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
}