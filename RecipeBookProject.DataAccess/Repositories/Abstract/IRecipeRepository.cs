using Azure.Core;
using Microsoft.EntityFrameworkCore.Query;
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
       Task<List<Product>> GetSearchedProductsRepositoryAsync(int? categoryid);
       Task<List<Category>> GetAllCategoriesRepository();
       Task AddAsync(User model);
       Task<Product?> GetDetailedRecipeRepository(int productId);
       Task<bool> SaveRecipeRepositoryAsync(int userId, int productId, bool isSaved);
       Task<bool> VoteRecipeRepositoryAsync(int userId, int productId, int vote);
       Task<bool> GetSavedRecipeRepositoryAsync(int userId, int productId);
       Task<(int totalVoters, double avg, int? userVote)> GetVotedRecipeRepositoryAsync(int userId, int productId);
       Task<IQueryable<Comment>> GetProductCommentsRepositoryAsync(int id);
       Task<bool> AddCommentsRepositoryAsync(int productid, int userid, bool isSecret,string comment);
       Task<bool> SaveAbuseRepositoryAsync(int userid, int ProductId, int AbuseCategoryId, string Description);
       Task<List<AbuseCategory>> GetAbuseCategoryRepositoryAsync();
        Task<bool> CreateRecipeRepositoryAsync(int userId,PendingProduct product);
    }
}
