namespace RecipeBookProject.Client.Models
{
    public class CreateProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductShortDesc { get; set; } = null!;
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int? ProductionTime { get; set; }
        public string ProductDetailedText { get; set; } = null!;
    }
}
