using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class AbuseRequestDto
    {
        public int ProductId { get; set; }
        public int AbuseCategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
