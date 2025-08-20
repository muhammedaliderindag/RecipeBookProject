using RecipeBookProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Abstract
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        Task<RefreshToken> GenerateAndStoreRefreshTokenAsync(int userId);
    }
}
