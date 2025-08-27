using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBookProject.Contracts.Recipes
{
    public class CreateProductDto
    {
        public int ProductId { get; set; }
        
        [Required, StringLength(200)]
        public string ProductName { get; set; } = null!;
        
        [Required, StringLength(500)]
        public string ProductShortDesc { get; set; } = null!;
        
        [Required]
        public int CategoryId { get; set; }
        
        public string ImageUrl { get; set; } = null!;
        
        [Range(0, 1440)]
        public int? ProductionTime { get; set; }
        
        [Required]
        public string ProductDetailedText { get; set; } = null!;
        
        [Range(1, 20)]
        public int BaseServingSize { get; set; } = 1;
        
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    }
}

