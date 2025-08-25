using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Contracts.Admin;

public class PendingProductsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    /// "pending" | "approved" | "all"
    public string Status { get; set; } = "pending";
    public int? CategoryId { get; set; }
    public string? Query { get; set; } // isim/kısa açıklama araması
}