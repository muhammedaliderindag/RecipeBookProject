namespace RecipeBookProject.Client.Models
{
    public class CommentDto
    {
        public int ProductId { get; set; }            
        public string UserDisplayName { get; set; } = default!;
        public string? UserAvatarUrl { get; set; }      
        public bool IsMine { get; set; }               
        public string Text { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
