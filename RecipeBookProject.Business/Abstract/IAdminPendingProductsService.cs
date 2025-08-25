using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Common;
namespace RecipeBookProject.Business.Abstract
{
    public interface IAdminPendingProductsService
    {
        Task<PagedResult<AdminPendingProductDto>> GetAsync(PendingProductsQuery input, CancellationToken ct = default);
        Task<bool> ApproveAsync(int productId, CancellationToken ct = default);
        Task<bool> RejectAsync(int productId, CancellationToken ct = default);
    }
}
