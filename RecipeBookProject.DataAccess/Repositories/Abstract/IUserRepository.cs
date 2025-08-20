using RecipeBookProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.DataAccess.Repositories.Abstract
{
    public interface IUserRepository
    {
       Task<User> GetUserByIdAsync(string email);
       Task AddAsync(User model);
        Task<User> GetUserByIdAsync(int? id);
        Task UpdateAsync(User entity);
    }
}
