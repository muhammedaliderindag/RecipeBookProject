using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
