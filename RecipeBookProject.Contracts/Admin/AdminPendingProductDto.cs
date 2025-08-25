using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Contracts.Admin;

public class AdminPendingProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductShortDesc { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public int ProductionTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsApproved { get; set; }
    public string Author { get; set; } = null!;
}