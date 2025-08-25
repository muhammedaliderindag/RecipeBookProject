using System.Net.Http.Json;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Common;

namespace RecipeBookProject.Client.Services;

public class AdminProductsService
{
    private readonly HttpClient _http;
    public AdminProductsService(HttpClient http) => _http = http;

    public async Task<PagedResult<AdminPendingProductDto>> GetAsync(
        int page, int pageSize, string status = "pending", int? categoryId = null, string? query = null)
    {
        var url = $"api/admin/pending-products?page={page}&pageSize={pageSize}&status={status}";
        if (categoryId is > 0) url += $"&categoryId={categoryId}";
        if (!string.IsNullOrWhiteSpace(query)) url += $"&query={Uri.EscapeDataString(query)}";

        var res = await _http.GetFromJsonAsync<PagedResult<AdminPendingProductDto>>(url);
        return res ?? new PagedResult<AdminPendingProductDto>();
    }

    public Task<HttpResponseMessage> ApproveAsync(int id)
        => _http.PostAsync($"api/admin/pending-products/{id}/approve", content: null);

    public Task<HttpResponseMessage> RejectAsync(int id)
        => _http.PostAsync($"api/admin/pending-products/{id}/reject", content: null);
}