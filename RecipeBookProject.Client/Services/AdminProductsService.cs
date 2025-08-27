using System.Net.Http.Json;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Recipes;

namespace RecipeBookProject.Client.Services;

public class AdminProductsService(HttpClient _http)
{
    public Task<PagedResult<AdminPendingProductDto>?> GetAsync(PendingProductsQuery input, CancellationToken ct = default)
        => _http.GetFromJsonAsync<PagedResult<AdminPendingProductDto>>(
            $"api/admin/pending-products?page={input.Page}&pageSize={input.PageSize}&status={input.Status}&categoryId={input.CategoryId}&query={input.Query}", ct);

    public Task<HttpResponseMessage> ApproveAsync(int id)
        => _http.PostAsync($"api/admin/pending-products/{id}/approve", content: null);

    public Task<HttpResponseMessage> RejectAsync(int id)
        => _http.PostAsync($"api/admin/pending-products/{id}/reject", content: null);

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/admin/pending-products/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public Task<HttpResponseMessage> ToggleVisibilityAsync(int id)
        => _http.PostAsync($"api/admin/pending-products/{id}/toggle-visibility", content: null);

    public async Task<AdminDashboardDto?> GetDashboardAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<AdminDashboardDto>("api/admin/pending-products/dashboard", ct);
}