using RecipeBookProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.DataAccess.Repositories.Abstract
{
    public interface IPendingProductRepository : IRepository<PendingProduct>
    {
        // Gerekirse ek özel metod imzaları burada
        Task<PendingProduct?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
