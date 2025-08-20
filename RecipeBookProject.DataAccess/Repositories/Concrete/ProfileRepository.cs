using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Data.Context;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.DataAccess.Repositories.Concrete
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly RecipeBookProjectDbContext _context;

        public ProfileRepository(RecipeBookProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetSavedProductRepositoryAsync(int? userId)
        {
            return await _context.SavedProducts
                    .Where(sp => sp.UserId == userId)
                    .Include(sp => sp.Product)
                        .ThenInclude(p => p.Category)  // Include/ThenInclude projeksiyondan önce
                    .Select(sp => sp.Product)
                    .AsNoTracking()
                    .Distinct()
                    .ToListAsync();
        }

        public async Task<User> GetUserDetailRepositoryAsync(int? userid)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == userid.Value);
        }

        public Task<User> UpdateProfileDetailsRepositoryAsync(int? userid)
        {
            throw new NotImplementedException();
        }
    }
}
