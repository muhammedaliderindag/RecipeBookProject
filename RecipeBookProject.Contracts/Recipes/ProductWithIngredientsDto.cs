using System;
using System.Collections.Generic;

namespace RecipeBookProject.Contracts.Recipes
{
    public class ProductWithIngredientsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductShortDesc { get; set; } = null!;
        public string? ProductDetailedText { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public int? ProductionTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int BaseServingSize { get; set; } = 1;
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
        public string CategoryName { get; set; } = null!;
    }
}

