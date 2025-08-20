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
    public class UserRepository : IUserRepository
    {
        private readonly RecipeBookProjectDbContext _context;

        public UserRepository(RecipeBookProjectDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User model)
        {
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByIdAsync(int? id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task UpdateAsync(User entity)
        {
            var set = _context.Set<User>();

            // Aynı Id'li bir entity zaten context tarafından izleniyorsa onu güncelle
            var local = await set.FindAsync(entity.UserId);
            if (local is not null)
            {
                _context.Entry(local).CurrentValues.SetValues(entity);
            }
            else
            {
                // İzlenmiyorsa attach et ve komple modified işaretle
                set.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }


    }
}
