using RecipeBookProject.Data.Context;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBookProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace RecipeBookProject.DataAccess.Repositories.Concrete
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly RecipeBookProjectDbContext _context;

        public RefreshTokenRepository(RecipeBookProjectDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            // AuthService'in kullanıcı bilgisine ihtiyacı olabileceğinden,
            // ilişkili User verisini de çekmek için Include kullanıyoruz.
            return await _context.RefreshTokens
                                 .Include(rt => rt.User)
                                 .SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<RefreshToken?> GetRefreshTokenByIdAsync(int id)
        {
            // AuthService'in kullanıcı bilgisine ihtiyacı olabileceğinden,
            // ilişkili User verisini de çekmek için Include kullanıyoruz.

            // IsActive özelliğinin mantığını (Revoked == null && Expires > DateTime.UtcNow)
            // doğrudan LINQ sorgusuna ekliyoruz.
            return await _context.RefreshTokens
                                 .Include(rt => rt.User)
                                 .FirstOrDefaultAsync(rt => rt.UserId == id &&
                                                       rt.Revoked == null &&
                                                       rt.Expires > DateTime.UtcNow);
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
