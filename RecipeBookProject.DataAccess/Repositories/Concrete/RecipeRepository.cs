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
                .Where(p => p.IsVisible) // Sadece görünür tarifleri getir
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
                    FeaturedCategory = p.FeaturedCategory == null ? null : new FeaturedCategory
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
                .Where(p => p.IsVisible) // Sadece görünür tarifleri getir
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
                .Where(p => p.IsVisible) // Sadece görünür tarifleri getir
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
                    FeaturedCategory = p.FeaturedCategory == null ? null : new FeaturedCategory
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
                .Where(p => (categoryid == null || p.CategoryId == categoryid))
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductShortDesc = p.ProductShortDesc,
                    ProductDetailedText = p.ProductDetailedText,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    ProductionTime = p.ProductionTime,
                    FeaturedCategory = p.FeaturedCategory == null ? null : new FeaturedCategory
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

        public async Task<bool> AddCommentsRepositoryAsync(int productid, int userid, bool isSecret, string comment)
        {
            var entity = new Comment
            {
                ProductId = productid,
                UserId = userid,
                Secret = isSecret,
                Text = comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(entity);
            var affected = await _context.SaveChangesAsync(); 

            return affected > 0;
        }

        public async Task<List<AbuseCategory>> GetAbuseCategoryRepositoryAsync()
        {
            var list = await _context.AbuseCategories
                .Select(ac => new AbuseCategory
                {
                    CategoryId = ac.CategoryId,
                    CategoryName = ac.CategoryName
                })
                .ToListAsync();
            return list;
        }

        public async Task<bool> SaveAbuseRepositoryAsync(int userid, int ProductId, int AbuseCategoryId, string Description)
        {
            var entity = new ProductAbuse
            {
                ProductId = ProductId,
                UserId = userid,
                CategoryId = AbuseCategoryId,
                Text = Description,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ProductAbuses.AddAsync(entity);
            var affected = await _context.SaveChangesAsync();

            return affected > 0;
        }

        public async Task<int> CreateRecipeRepositoryAsync(int userId, PendingProduct product)
        {
            // Debug: ImageUrl kontrolü
            Console.WriteLine($"DEBUG: CreateRecipeRepositoryAsync - Product ImageUrl: {product.ImageUrl}");
            Console.WriteLine($"DEBUG: CreateRecipeRepositoryAsync - Product ImageUrl length: {product.ImageUrl?.Length}");
            Console.WriteLine($"DEBUG: CreateRecipeRepositoryAsync - Product ImageUrl contains extension: {product.ImageUrl?.Contains(".")}");
            
            await _context.PendingProducts.AddAsync(product);
            var affected = await _context.SaveChangesAsync();

            if (affected > 0)
            {
                Console.WriteLine($"DEBUG: CreateRecipeRepositoryAsync - Successfully saved with ID: {product.ProductId}");
                Console.WriteLine($"DEBUG: CreateRecipeRepositoryAsync - Saved ImageUrl: {product.ImageUrl}");
                return product.ProductId; // Kaydedilen PendingProduct'ın ID'sini döndür
            }
            
            Console.WriteLine($"ERROR: CreateRecipeRepositoryAsync - Failed to save product");
            return 0; // Kaydetme başarısız
        }

        public async Task<Product?> GetByIdAsync(int productId, CancellationToken ct)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId, ct);
        }

        public async Task<List<RecipeIngredient>> GetRecipeIngredientsRepositoryAsync(int productId, CancellationToken ct = default)
        {
            try
            {
                // Null check ekle
                if (productId <= 0)
                {
                    Console.WriteLine($"DEBUG: GetRecipeIngredientsRepositoryAsync - Invalid productId: {productId}");
                    return new List<RecipeIngredient>();
                }

                Console.WriteLine($"DEBUG: GetRecipeIngredientsRepositoryAsync - Searching for productId: {productId}");
                
                var result = await _context.RecipeIngredients
                    .Include(ri => ri.Ingredient)
                    .Where(ri => ri.ProductId == productId)  // ProductId null olabilir
                    .ToListAsync(ct);
                
                Console.WriteLine($"DEBUG: GetRecipeIngredientsRepositoryAsync - Found {result.Count} ingredients");
                
                // Her malzeme için null check yap
                foreach (var ri in result)
                {
                    if (ri.Ingredient == null)
                    {
                        Console.WriteLine($"DEBUG: GetRecipeIngredientsRepositoryAsync - Ingredient is null for RecipeIngredientId: {ri.RecipeIngredientId}");
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: GetRecipeIngredientsRepositoryAsync - Exception: {ex.Message}");
                Console.WriteLine($"ERROR: Stack trace: {ex.StackTrace}");
                return new List<RecipeIngredient>();
            }
        }
        
        public async Task<List<RecipeIngredient>> GetPendingRecipeIngredientsRepositoryAsync(int pendingProductId, CancellationToken ct = default)
        {
            try
            {
                // Null check ekle
                if (pendingProductId <= 0)
                {
                    Console.WriteLine($"DEBUG: GetPendingRecipeIngredientsRepositoryAsync - Invalid pendingProductId: {pendingProductId}");
                    return new List<RecipeIngredient>();
                }

                Console.WriteLine($"DEBUG: GetPendingRecipeIngredientsRepositoryAsync - Searching for pendingProductId: {pendingProductId}");
                
                var result = await _context.RecipeIngredients
                    .Include(ri => ri.Ingredient)
                    .Where(ri => ri.PendingProductId == pendingProductId)
                    .ToListAsync(ct);
                
                Console.WriteLine($"DEBUG: GetPendingRecipeIngredientsRepositoryAsync - Found {result.Count} ingredients");
                
                // Her malzeme için null check yap
                foreach (var ri in result)
                {
                    if (ri.Ingredient == null)
                    {
                        Console.WriteLine($"DEBUG: GetPendingRecipeIngredientsRepositoryAsync - Ingredient is null for RecipeIngredientId: {ri.RecipeIngredientId}");
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: GetPendingRecipeIngredientsRepositoryAsync - Exception: {ex.Message}");
                Console.WriteLine($"ERROR: Stack trace: {ex.StackTrace}");
                return new List<RecipeIngredient>();
            }
        }
        
        public async Task<bool> SaveRecipeIngredientsAsync(List<RecipeIngredient> recipeIngredients)
        {
            try
            {
                Console.WriteLine($"DEBUG: SaveRecipeIngredientsAsync called with {recipeIngredients.Count} ingredients");
                foreach (var ri in recipeIngredients)
                {
                    Console.WriteLine($"DEBUG: RecipeIngredient - PendingProductId: {ri.PendingProductId}, IngredientId: {ri.IngredientId}, Quantity: {ri.Quantity}");
                }
                
                await _context.RecipeIngredients.AddRangeAsync(recipeIngredients);
                var affected = await _context.SaveChangesAsync();
                
                Console.WriteLine($"DEBUG: SaveRecipeIngredientsAsync saved {affected} records");
                return affected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving recipe ingredients: {ex.Message}");
                Console.WriteLine($"Error details: {ex}");
                return false;
            }
        }
    }
}
