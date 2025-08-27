using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class SaveRecipeRequestDto
    {
        public int ProductId { get; set; }
        public bool IsSaved { get; set; }
    }
}
