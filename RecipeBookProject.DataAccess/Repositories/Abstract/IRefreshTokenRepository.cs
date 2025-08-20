using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBookProject.Data.Entities;
namespace RecipeBookProject.DataAccess.Repositories.Abstract
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<RefreshToken> AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenByIdAsync(int id);
    }
}
