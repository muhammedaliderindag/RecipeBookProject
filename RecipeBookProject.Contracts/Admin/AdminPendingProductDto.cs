using System;
using System.Collections.Generic;
using RecipeBookProject.Contracts.Recipes;

namespace RecipeBookProject.Contracts.Admin
{
    public class AdminPendingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductShortDesc { get; set; } = null!;
        public string ProductDetailedText { get; set; } = null!;
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int ProductionTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int BaseServingSize { get; set; } = 1;
        public bool IsApproved { get; set; } // Onay durumu
        public DateTime? ApprovedAt { get; set; } // Onaylanma tarihi
        public List<RecipeIngredientDto> Ingredients { get; set; } = new List<RecipeIngredientDto>();
    }
}