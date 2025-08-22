using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class AddCommentDto
    {
        public bool isSecret { get; set; }
        public string commentText { get; set; } = string.Empty;
    }
}
