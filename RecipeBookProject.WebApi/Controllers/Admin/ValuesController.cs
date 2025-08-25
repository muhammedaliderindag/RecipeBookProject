using Microsoft.AspNetCore.Mvc;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Common;

namespace RecipeBookProject.Api.Controllers;

[ApiController]
[Route("api/admin/pending-products")]
public class AdminProductsController : ControllerBase
{
    private readonly IAdminPendingProductsService _svc;
    public AdminProductsController(IAdminPendingProductsService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminPendingProductDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string status = "pending",
        [FromQuery] int? categoryId = null,
        [FromQuery] string? query = null,
        CancellationToken ct = default)
    {
        var input = new PendingProductsQuery
        {
            Page = page,
            PageSize = pageSize,
            Status = status,
            CategoryId = categoryId,
            Query = query
        };

        var result = await _svc.GetAsync(input, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        var ok = await _svc.ApproveAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        var ok = await _svc.RejectAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}