namespace RecipeBookProject.Contracts.Recipes
{
    public class RecipeIngredientDto
    {
        public int RecipeIngredientId { get; set; }
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public string? Notes { get; set; }
        public int BaseServingSize { get; set; } = 1;
    }
}

