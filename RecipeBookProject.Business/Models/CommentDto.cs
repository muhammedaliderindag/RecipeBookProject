using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public sealed record CommentDto
    {
        public int ProductId { get; init; }            
        public string UserDisplayName { get; init; } = default!;
        public string? UserAvatarUrl { get; init; }      
        public bool IsMine { get; init; }               
        public string Text { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
    }
}
