using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public List<ProductDto> ProductList { get; set; } = new List<ProductDto>();

    }
}
