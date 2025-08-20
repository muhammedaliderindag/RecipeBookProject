using RecipeBookProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.DataAccess.Repositories.Abstract
{
    public interface IRecipeRepository
    {
       Task<List<Product>> GetAllShortRecipeRepositoryAsync();
       Task<List<Product>> GetSearchedProductsRepository(string query,int? categoryid);
       Task<List<Category>> GetAllCategoriesRepository();
       Task AddAsync(User model);
       Task<Product?> GetDetailedRecipeRepository(int productId);
       Task<IQueryable<Comment>> GetProductCommentsRepositoryAsync(int id);
    }
}
