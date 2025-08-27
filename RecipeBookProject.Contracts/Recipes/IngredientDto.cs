using System.ComponentModel.DataAnnotations;

namespace RecipeBookProject.Contracts.Recipes
{
    public class IngredientDto
    {
        public int IngredientId { get; set; }
        
        [Required(ErrorMessage = "Malzeme adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Malzeme adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = null!;
        
        [StringLength(50, ErrorMessage = "Birim en fazla 50 karakter olabilir.")]
        public string? DefaultUnit { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}

