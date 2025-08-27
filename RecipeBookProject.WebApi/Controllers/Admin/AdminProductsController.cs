using Microsoft.AspNetCore.Mvc;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Contracts.Admin;
using RecipeBookProject.Contracts.Recipes;

namespace RecipeBookProject.WebApi.Controllers.Admin
{
    [Route("api/admin/pending-products")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly IAdminPendingProductsService _svc;

        public AdminProductsController(IAdminPendingProductsService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AdminPendingProductDto>>> GetAsync([FromQuery] PendingProductsQuery input, CancellationToken ct = default)
        {
            var result = await _svc.GetAsync(input, ct);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard([FromQuery] int days = 7, CancellationToken ct = default)
        {
            var result = await _svc.GetDashboardAsync(days, ct);
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult> ApproveAsync(int id, CancellationToken ct = default)
        {
            var result = await _svc.ApproveAsync(id, ct);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult> RejectAsync(int id, CancellationToken ct = default)
        {
            var result = await _svc.RejectAsync(id, ct);
            return result ? Ok() : BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductAsync(int id, [FromBody] UpdateProductDto dto, CancellationToken ct = default)
        {
            var result = await _svc.UpdateProductAsync(id, dto, ct);
            return result ? Ok(new { message = "Tarif başarıyla güncellendi." }) : BadRequest(new { message = "Tarif güncellenemedi." });
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<ActionResult> ToggleVisibilityAsync(int id, CancellationToken ct = default)
        {
            var result = await _svc.ToggleProductVisibilityAsync(id, ct);
            return result ? Ok(new { message = "Tarif görünürlüğü başarıyla değiştirildi." }) : BadRequest(new { message = "Görünürlük değiştirilemedi." });
        }
    }
}



