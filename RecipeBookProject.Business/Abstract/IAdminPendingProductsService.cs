using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Recipes;

namespace RecipeBookProject.Business.Abstract;

public interface IAdminPendingProductsService
{
	Task<PagedResult<AdminPendingProductDto>> GetAsync(PendingProductsQuery input, CancellationToken ct = default);
	Task<bool> ApproveAsync(int productId, CancellationToken ct = default);
	Task<bool> RejectAsync(int productId, CancellationToken ct = default);
	Task<AdminDashboardDto> GetDashboardAsync(int days, CancellationToken ct = default);
	Task<bool> UpdateProductAsync(int productId, UpdateProductDto dto, CancellationToken ct = default);
	Task<bool> ToggleProductVisibilityAsync(int productId, CancellationToken ct = default);
}
