using RecipeBookProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RecipeBookProject.DataAccess.Repositories.Abstract
{
    public interface IProfileRepository
    {
        Task<List<Product>> GetSavedProductRepositoryAsync(int? userid);
        Task<User> GetUserDetailRepositoryAsync(int? userid);
        Task<User> UpdateProfileDetailsRepositoryAsync(int? userid);
    }
}
