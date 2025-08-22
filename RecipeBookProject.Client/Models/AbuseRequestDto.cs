namespace RecipeBookProject.Client.Models
{
    public class AbuseRequestDto
    {
        public int ProductId { get; set; }
        public int AbuseCategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
