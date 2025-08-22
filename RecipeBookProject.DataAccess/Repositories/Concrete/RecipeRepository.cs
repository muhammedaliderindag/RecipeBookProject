using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Data.Context;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecipeBookProject.DataAccess.Repositories.Concrete
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeBookProjectDbContext _context;

        public RecipeRepository(RecipeBookProjectDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User model)
        {
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllCategoriesRepository()
        {
            return await _context.Categories.Select(p => new Category
            {
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName,
            }).ToListAsync();
        }

        public async Task<List<Product>> GetAllShortRecipeRepositoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.FeaturedCategory)
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductShortDesc = p.ProductShortDesc,
                    ProductDetailedText = p.ProductDetailedText,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    ProductionTime = p.ProductionTime,
                    FeaturedCategory = new FeaturedCategory
                    {
                        FeaturedCategoryId = p.FeaturedCategory.FeaturedCategoryId,
                        FeaturedCategoryName = p.FeaturedCategory.FeaturedCategoryName
                    },
                    Category = new Category
                    {
                        CategoryId = p.Category.CategoryId,
                        CategoryName = p.Category.CategoryName
                    }
                })
                .ToListAsync();
        }

        public async Task<Product?> GetDetailedRecipeRepository(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public Task<IQueryable<Comment>> GetProductCommentsRepositoryAsync(int productId)
        {
            IQueryable<Comment> q = _context.Comments
                .AsNoTracking()
                .Where(c => c.ProductId == productId);

            // Kullanıcı alanlarına ihtiyacın varsa şunu aç:
             q = q.Include(c => c.User);

            return Task.FromResult(q);
        }



        public Task<List<Product>> GetSearchedProductsRepository(string query, int? categoryid)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.FeaturedCategory)
                .Where(p =>
                        ((EF.Functions.Like(p.ProductName, "%" + query + "%") ||
                         EF.Functions.Like(p.ProductDetailedText, "%" + query + "%")))
                        && (categoryid == null || p.CategoryId == categoryid)
)
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductShortDesc = p.ProductShortDesc,
                    ProductDetailedText = p.ProductDetailedText,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    ProductionTime = p.ProductionTime,
                    FeaturedCategory = new FeaturedCategory
                    {
                        FeaturedCategoryId = p.FeaturedCategory.FeaturedCategoryId,
                        FeaturedCategoryName = p.FeaturedCategory.FeaturedCategoryName
                    },
                    Category = new Category
                    {
                        CategoryId = p.Category.CategoryId,
                        CategoryName = p.Category.CategoryName
                    }
                }).ToListAsync();
        }

        public async Task<bool> GetSavedRecipeRepositoryAsync(int userId, int productId)
        {
            return await _context.SavedProducts
                .AnyAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        public async Task<bool> SaveRecipeRepositoryAsync(int userId, int productId, bool isSaved)
        {
            var existing = await _context.SavedProducts
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (isSaved)
            {
                if (existing == null)
                {
                    var entity = new SavedProduct
                    {
                        UserId = userId,
                        ProductId = productId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.SavedProducts.Add(entity);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                if (existing != null)
                {
                    _context.SavedProducts.Remove(existing);
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }

        public Task<List<Product>> GetSearchedProductsRepositoryAsync(int? categoryid)
        {
            return _context.Products
    .Include(p => p.Category)
    .Include(p => p.FeaturedCategory)
    .Where(p =>(categoryid == null || p.CategoryId == categoryid))
    .Select(p => new Product
    {
        ProductId = p.ProductId,
        ProductName = p.ProductName,
        ProductShortDesc = p.ProductShortDesc,
        ProductDetailedText = p.ProductDetailedText,
        CategoryId = p.CategoryId,
        ImageUrl = p.ImageUrl,
        ProductionTime = p.ProductionTime,
        FeaturedCategory = new FeaturedCategory
        {
            FeaturedCategoryId = p.FeaturedCategory.FeaturedCategoryId,
            FeaturedCategoryName = p.FeaturedCategory.FeaturedCategoryName
        },
        Category = new Category
        {
            CategoryId = p.Category.CategoryId,
            CategoryName = p.Category.CategoryName
        }
    }).ToListAsync();
        }

        public async Task<bool> VoteRecipeRepositoryAsync(int userId, int productId, int vote)
        {
            // Önceden oy var mı diye bak
            var existing = await _context.ProductVotes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (existing == null)
            {
                var newVote = new ProductVote
                {
                    UserId = userId,
                    ProductId = productId,
                    Vote = vote,        
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ProductVotes.AddAsync(newVote);
            }
            else
            {
                existing.Vote = vote;
                existing.CreatedAt = DateTime.UtcNow;

                _context.ProductVotes.Update(existing);
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(int totalVoters, double avg, int? userVote)> GetVotedRecipeRepositoryAsync(int userId, int productId)
        {
            var q = _context.ProductVotes.Where(x => x.ProductId == productId);

            // Kullanıcının kendi oyu
            var userVote = await q
                .Where(x => x.UserId == userId)
                .Select(x => (int?)x.Vote)
                .FirstOrDefaultAsync();

            // Toplam kişi sayısı
            var totalVoters = await q.CountAsync();

            // Ortalama
            double avg = 0.0;
            if (totalVoters > 0)
                avg = await q.AverageAsync(x => (double)x.Vote);

            return (totalVoters, avg, userVote);
        }
    }
}
